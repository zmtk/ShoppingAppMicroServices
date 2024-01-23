import { Button, Col, Input, InputGroup, Row } from "reactstrap"
import { getProductThumbnail } from "../catalog/components/productImage"
import { useDeleteProductMutation, useSetProductQuantityMutation } from "./basketApiSlice"
import { useState } from "react"
import { currTRY } from "../../helpers/currencyFormatter"
import { useCallback } from "react"
import { toast } from "react-toastify"

export function BasketItem(props) {

    const [quantity, setQuantity] = useState(props.product.quantity);

    const [setProductQuantityMutation] = useSetProductQuantityMutation();
    const [deleteProductMutation] = useDeleteProductMutation();

    const { product, refetchBasket } = props;
    const { productId } = product;


    const deleteProduct = useCallback(async () => {
        try {
            await deleteProductMutation(productId).unwrap();
            await refetchBasket()
            toast.success(
                `${props.product.name} is removed from basket`
              );
        } catch (err) {
            console.log(err);
        }
    }, [deleteProductMutation, props.product.name, refetchBasket, productId]); // Include the specific dependencies


    const setProductQuantity = async (newQuantity) => {

        setQuantity(newQuantity);

        try {
            const Response = await setProductQuantityMutation(
                {
                    productId: props.product.productId,
                    quantity: newQuantity
                }).unwrap()

            setQuantity(Response.updatedQuantity);
            await refetchBasket()
        } catch (err) {
            console.log(err)
        }

    }

    const handleSetQuantityInput = (e) => setProductQuantity(parseInt(e.target.value, 10))
    const handleIncreaseQuantity = () => setProductQuantity(quantity + 1)
    const handleDecreaseQuantity = () => setProductQuantity(quantity - 1)


    return (
        <Row className={`d-flex align-items-center mb-1 ${props.product.inactive ? 'disabled-row' : ''}`}>
            <Col lg={5} md={5}>
                <Row className="d-flex align-items-center">
                    <Col lg={4} md={5}>
                        <img className="product-thumb" alt={props.product.name} src={getProductThumbnail(props.product.productId)} />
                    </Col>
                    <Col lg={8} md={7}>
                        <div className="product-info"><span> {props.product.name}</span></div>
                    </Col>
                </Row>
            </Col>
            <Col md={1}><span>{currTRY.format(props.product.price)}</span></Col>
            <Col md={2}>

                <InputGroup className="d-flex justify-content-center">
                    <Button className="xsmall-btn" onClick={handleDecreaseQuantity} color="danger" outline>-</Button>
                    <Input className="xsmall-input text-center" onChange={handleSetQuantityInput} value={quantity}></Input>
                    <Button className="xsmall-btn" onClick={handleIncreaseQuantity} color="success" outline>+</Button>
                </InputGroup>

            </Col>

            <Col lg={1} md={1}><span>{currTRY.format(props.product.total)}</span></Col>
            <Col lg={3} md={3}><Button block size="sm" onClick={deleteProduct} color="danger">remove</Button></Col>
        </Row>
    )

}
