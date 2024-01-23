import {
    Card,
    CardBody,
    Dropdown,
    DropdownToggle,
    DropdownMenu,
    DropdownItem,
    CardTitle,
    Button,
    Col,
    Row,
} from "reactstrap";
import { useState, useEffect } from "react";
import AddStore from "./AddStore";
import { useDeleteStoreMutation } from "../storeApiSlice";
import { toast } from "react-toastify";

const StoreList = ({ stores, refetchStores, selectedStoreId, onSelectStore }) => {

    const [selectedStore, setSelectedStore] = useState(null);
    const [dropdownOpen, setDropdownOpen] = useState(false);

    const [DeleteStore] = useDeleteStoreMutation()

    const [addStoreModal, setAddStoreModal] = useState(false);
    const toggleAddStoreModal = () => setAddStoreModal(!addStoreModal);

    useEffect(() => {
        const store = stores.find((store) => store.id === selectedStoreId);
        setSelectedStore(store);
    }, [selectedStoreId, stores]);

    const toggleDropdown = () => setDropdownOpen(!dropdownOpen);

    const handleDeleteStore = async () => {
        console.log(selectedStoreId);
        try {

            const response = await DeleteStore(selectedStoreId).unwrap();

            await refetchStores();
            toast.success(
                `${selectedStore.name} Store deleted successfully`,
                {
                    toastId: 'storedeleted',
                }
            );
            console.log(response);
        } catch (err) {
            console.error('Error deleting product:', err.message);
        }
    };

    return (
        <>
            <AddStore addStoreModal={addStoreModal} refetchStores={refetchStores} toggleAddStoreModal={toggleAddStoreModal} />
            <Card className="p-3">
                <CardBody>
                    <Row>
                        <Col>
                            <Dropdown isOpen={dropdownOpen} toggle={toggleDropdown}>
                                <DropdownToggle
                                    caret
                                    style={{
                                        backgroundColor: "transparent",
                                        border: "none",
                                        boxShadow: "none",
                                        color: "#333", // You can set the color you prefer
                                        fontSize: "1.5rem", // You can adjust the font size
                                    }}
                                >
                                    {selectedStore ? selectedStore.name : "Select a Store"}
                                </DropdownToggle>
                                <DropdownMenu>
                                    {stores.map((store) => (
                                        !store.inactive && (
                                            <DropdownItem
                                                key={store.id}
                                                onClick={() => onSelectStore(store.id)}
                                            >
                                                {store.name}
                                            </DropdownItem>
                                        )
                                    ))}

                                </DropdownMenu>
                            </Dropdown>
                            {selectedStore && (
                                <div>
                                    <CardTitle tag="h2" onClick={() => toggleDropdown()}>
                                    </CardTitle>
                                </div>
                            )}
                        </Col>
                        <Col md="3">
                            <Row className="mb-2">
                                <Button className="btn-sm" color="success" onClick={toggleAddStoreModal}>Add Store</Button>
                            </Row>

                            <Row className="mb-2">
                                <Button className="btn-sm" color="danger" onClick={handleDeleteStore}>Delete Store</Button>
                            </Row>
                        </Col>

                    </Row>
                </CardBody>
            </Card>
        </>
    );
};

export default StoreList;