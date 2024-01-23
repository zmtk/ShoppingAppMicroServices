import { useCallback, useState } from "react";
import { Modal, ModalHeader, ModalBody, ModalFooter, Col, Button, Input, Label, FormGroup, } from "reactstrap";
import { useAddStoreMutation } from "../storeApiSlice";
import { toast } from "react-toastify";

const AddStore = ({ addStoreModal, refetchStores, toggleAddStoreModal }) => {

    const [newStore, setNewStore] = useState({
        name: ''
    });

    const [AddStore] = useAddStoreMutation()

    const handleUpdateStoreState = useCallback(
        (event) => {
            const { name, value } = event.target;
            setNewStore((prevAddress) => ({ ...prevAddress, [name]: value }));
            console.log(name, value);
        },
        [setNewStore]
    );


    const handleAddStore = async () => {
        try {
            const response = await AddStore(newStore).unwrap();
            toggleAddStoreModal();
            console.log(response);
            toast.success(
                `New Store added successfully`,
                {
                    toastId: 'storeadded',
                }
            );
            refetchStores();
        } catch (err) {
            console.error('Error deleting product:', err.message);
        }
    };
    return (
        <Modal isOpen={addStoreModal} toggle={toggleAddStoreModal}>
            <ModalHeader toggle={toggleAddStoreModal}>Add New Store</ModalHeader>
            <ModalBody>
                <FormGroup>
                    <Label for="storeName">Store Name</Label>
                    <Input type="text" name="name" id="storeName" onChange={handleUpdateStoreState} value={newStore.storeName} placeholder="Enter store name" />
                </FormGroup>
            </ModalBody>
            <ModalFooter>
                <Col>
                    <Button color="success" onClick={handleAddStore}>
                        Add New Store
                    </Button>{' '}
                </Col>
                <Button color="secondary" onClick={toggleAddStoreModal}>
                    Cancel
                </Button>
            </ModalFooter>
        </Modal>
    );
}

export default AddStore;