import React, { useState } from "react";
import { Card, CardBody, CardSubtitle, CardText, CardTitle, Col, Button, Badge } from "reactstrap";
import { getProductImageUrl } from "./productImage";
import { currTRY } from "../../../helpers/currencyFormatter";
import UpdateProduct from "./UpdateProduct";

export function Product(props) {
    const [updateProductModal, setUpdateProductModal] = useState(false);
    const toggleUpdateProductModal = () => setUpdateProductModal(!updateProductModal);

    const [isHovered, setIsHovered] = useState(false);

    // Placeholder function, replace with your actual edit logic
    const handleEditProduct = (productId) => {
        toggleUpdateProductModal();
        console.log(updateProductModal);
        console.log(`Edit product with ID: ${productId}`);

        // Implement your edit logic here, e.g., open a modal or navigate to an edit page
    };

    return (
        <>

            <UpdateProduct
                updateProductModal={updateProductModal}
                setUpdateProductModal={setUpdateProductModal}
                toggleUpdateProductModal={toggleUpdateProductModal}
                refetchProducts={props.refetchProducts}
                product={props.product}
            />

            <Col className="mb-4">
                <Card
                    className={`Card ${isHovered ? "hovered-card" : ""} ${props.product.inactive ? "disabled-card" : ""}`}
                    onMouseEnter={() => setIsHovered(true)}
                    onMouseLeave={() => setIsHovered(false)}
                >
                    <img
                        src={getProductImageUrl(props.product.id)}
                        alt={props.product.name}
                    />
                    <CardBody>
                        <CardTitle tag="h5" >
                            <span>{props.product.name}</span>
                            {props.product.inactive ? (
                            <Badge className="mt-2" color="secondary">
                                Disabled
                            </Badge>
                            ) : null}
                        </CardTitle>
                        <CardSubtitle tag="b">
                            <span>{props.product.brand}</span>
                        </CardSubtitle>
                        <CardText >
                            <span>{props.product.gender} {props.product.type}</span>
                        </CardText>
                        <CardText tag="b">
                            <span>{currTRY.format(props.product.price)}</span>
                        </CardText>

                        <Button
                            color="primary"
                            className="edit-button"
                            onClick={() => handleEditProduct(props.product.id)}
                        >
                            Edit Product
                        </Button>
                    </CardBody>
                </Card>
            </Col>
        </>

    );



}

