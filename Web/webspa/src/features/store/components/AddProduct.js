import ProductInput from "./ProductInput";
import { Row, Col, Modal, ModalBody, ModalHeader, ModalFooter, Button } from "reactstrap";
import { useState, useCallback, useEffect } from "react";
import { useAddProductMutation } from "../storeApiSlice";
import { toast } from "react-toastify";

const AddProduct = ({ addProductModal, setAddProductModal, refetchProducts, toggleAddProductModal,  selectedStoreId }) => {
    
    const [AddProduct] = useAddProductMutation()

    const [newProduct, setNewProduct] = useState({
        name: '', type: '', brand: '', gender: '', price: '', storeId: selectedStoreId
    });

    useEffect(() => {
        setNewProduct((prevProduct) => ({
            ...prevProduct,
            storeId: selectedStoreId,
        }));
    }, [selectedStoreId]);
    
    const handleAddNewProduct = async () => {
        console.log(newProduct);
        try {
            const Response = await AddProduct({ ...newProduct }).unwrap()

            await refetchProducts();
            toast.success(
                `New Product added successfully`,
                {
                    toastId: 'productadded',
                }
            );
            setAddProductModal(false);
            console.log(Response);
        } catch (err) {
            console.log(err.message);
        }
    }

    const handleNewProduct = useCallback(
        (event) => {
            const { name, value } = event.target;
            setNewProduct((prevAddress) => ({ ...prevAddress, [name]: value }));
            console.log(name, value);
        },
        [setNewProduct]
    );

    return (
        <Modal isOpen={addProductModal} toggle={toggleAddProductModal}>
            <ModalHeader toggle={toggleAddProductModal}>Add New Product</ModalHeader>
            <ModalBody>
                <>
                    <ProductInput
                        label="Name"
                        name="name"
                        onChange={handleNewProduct}
                    />

                    <Row>
                        <Col md={6}>
                            <ProductInput
                                label="Type"
                                name="type"
                                onChange={handleNewProduct}
                            />
                        </Col>
                        <Col md={6}>
                            <ProductInput
                                label="Brand"
                                name="brand"
                                onChange={handleNewProduct}
                            />
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <ProductInput
                                label="Gender"
                                name="gender"
                                onChange={handleNewProduct}
                            />
                        </Col>
                        <Col md={6}>
                            <ProductInput
                                name="price"
                                onChange={handleNewProduct}
                            />
                        </Col>
                    </Row>
                </>
            </ModalBody>
            <ModalFooter>
                <Button color="primary" onClick={handleAddNewProduct}>
                    Add New Product
                </Button>{' '}
                <Button color="secondary" onClick={toggleAddProductModal}>
                    Cancel
                </Button>
            </ModalFooter>
        </Modal>
    )
}

export default AddProduct;