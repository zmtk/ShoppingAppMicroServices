import { Button, Card, CardSubtitle, CardText, CardTitle, Col, Row } from "reactstrap";
import { getProductDetailImageUrl } from "./productImage";
import { useAddToBasketMutation } from "../catalogApiSlice";
// import { useAddToBasketMutation } from "./catalogApiSlice";
import { currTRY } from "../../../helpers/currencyFormatter";
import { useGetBasketQuery } from "../../basket/basketApiSlice";
import { toast } from 'react-toastify';

export function ProductDetail(props) {
    const { refetch: refetchBasket } = useGetBasketQuery();
    const [addToBasket] = useAddToBasketMutation()

    const notify = () => toast.success(`${props.product.brand + " "+ props.product.name} is added to basket`);
    
    const handleAddToBasket = async () => {
        try {
            await addToBasket(
                {
                    ProductId: props.product.id,
                    BasketEvent: "Add_Product_To_Basket",
                }).unwrap()

            await refetchBasket();
            notify();
        } catch (err) {
            console.log(err)
        }

    }

    return (
        <Row>

            <Col sm="6">
                <Card>
                    <img
                        className="Card-Image"
                        src={getProductDetailImageUrl(props.product.id)}
                        alt={props.product.name}
                    />
                </Card>
            </Col>


            <Col sm="5">
                <Card className="p-3">
                    <CardTitle tag="h5">
                        {props.product.name}
                    </CardTitle>
                    <CardSubtitle tag="b">
                        {props.product.brand}
                    </CardSubtitle>
                    <CardText>
                        {props.product.gender} {props.product.type}
                    </CardText>
                    <CardText tag="b" className="mt-5">
                        {currTRY.format(props.product.price)}
                    </CardText>
                    <div className="btn-group mt-2">
                        {props.product.inactive
                            ? (
                                <Button disabled size="sm" color="secondary">
                                    Out of stock
                                </Button>
                            ) : (
                                <Button size="sm" color="primary" onClick={handleAddToBasket}>
                                    Add To Card
                                </Button>
                            )}
                    </div>
                </Card>
            </Col>
        </Row >

    )
}
