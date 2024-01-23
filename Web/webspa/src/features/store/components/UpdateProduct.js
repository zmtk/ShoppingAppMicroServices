import ProductInput from "./ProductInput";
import { Modal, ModalBody, ModalHeader, ModalFooter, Button, Card, CardBody, CardTitle, CardSubtitle, CardText, Col } from "reactstrap";
import { currTRY } from "../../../helpers/currencyFormatter";
import { getProductImageUrl } from "./productImage";
import { useState, useCallback, useEffect } from "react";
import { useToggleStoreProductMutation, useUpdateProductMutation } from "../storeApiSlice";
import { toast } from "react-toastify";

const UpdateProduct = ({ updateProductModal, setUpdateProductModal, toggleUpdateProductModal, refetchProducts, product }) => {

    const [UpdateProduct] = useUpdateProductMutation()
    const [toggleStoreProduct] = useToggleStoreProductMutation()

    const [updateProduct, setUpdateProduct] = useState({
        id: product.id, newPrice: ''
    });

    useEffect(() => {
        setUpdateProduct((prevProduct) => ({
            ...prevProduct,
            id: product.id,
        }));
    }, [product.id]);

    const handleUpdateProduct = async () => {
        try {
            const updatedProduct = {
                id: updateProduct.id,
                newPrice: parseFloat(updateProduct.newPrice.replace(/,/g, '.')), // Convert to float
            };

            const response = await UpdateProduct(updatedProduct).unwrap();

            await refetchProducts();
            toast.success(
                `Product updated successfully`,
                {
                    toastId: 'productupdated',
                }
            );
            setUpdateProductModal(false);

            console.log(response);
        } catch (err) {
            console.error('Error updating product:', err.message);
        }
    };

    const handleDeleteProduct = async () => {

        try {

            const response = await toggleStoreProduct(updateProduct.id).unwrap();

            await refetchProducts();
            toast.success(
                `${response.message}`,
                {
                    toastId: 'productdisabled',
                }
            );

            setUpdateProductModal(false);

            console.log(response);
        } catch (err) {
            console.error('Error deleting product:', err.message);
        }
    };

    const handleUpdateProductState = useCallback(
        (event) => {
            const { name, value } = event.target;
            const sanitizedValue = value.replace(/,/g, '.'); // Replace commas with dots globally
            setUpdateProduct((prevAddress) => ({ ...prevAddress, [name]: sanitizedValue }));
            console.log(name, sanitizedValue);
        },
        [setUpdateProduct]
    );

    return (
        <Modal isOpen={updateProductModal} toggle={toggleUpdateProductModal}>
            <ModalHeader toggle={toggleUpdateProductModal}>Modal title</ModalHeader>
            <ModalBody>
                <>
                    <Card>
                        <img
                            src={getProductImageUrl(product.id)}
                            alt={product.name}
                        />
                        <CardBody>
                            <CardTitle tag="h5">
                                {product.name}
                            </CardTitle>
                            <CardSubtitle tag="b" >
                                {product.brand}
                            </CardSubtitle>
                            <CardText >
                                {product.gender} {product.type}
                            </CardText>
                            <CardText tag="b">
                                {currTRY.format(product.price)}
                            </CardText>
                            <ProductInput
                                name="newPrice"
                                onChange={handleUpdateProductState}
                            />
                        </CardBody>

                    </Card>

                </>
            </ModalBody>
            <ModalFooter>
                <Col>
                    {product.inactive ? (
                        <Button color="success" onClick={handleDeleteProduct}>
                        Enable Product
                        </Button>
                    ) : (
                        <Button color="danger" onClick={handleDeleteProduct}>
                        Disable Product
                        </Button>
                    )}
                    
                </Col>
                <Button color="primary" onClick={handleUpdateProduct}>
                    Update Price
                </Button>{' '}
                <Button color="secondary" onClick={toggleUpdateProductModal}>
                    Cancel
                </Button>
            </ModalFooter>
        </Modal>
    )
}

export default UpdateProduct;