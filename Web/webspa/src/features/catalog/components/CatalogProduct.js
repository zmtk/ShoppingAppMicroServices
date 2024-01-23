import { Button, Card, CardBody, CardSubtitle, CardText, CardTitle, Col } from "reactstrap";
import { getProductImageUrl } from "./productImage";
import { useAddToBasketMutation } from "../catalogApiSlice";
// import { useAddToBasketMutation } from "./catalogApiSlice";
import { currTRY } from "../../../helpers/currencyFormatter";
import { useGetBasketQuery } from "../../basket/basketApiSlice";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";

export function CatalogProduct(props) {

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
        <Col className="mb-4">
            <Card>
                <Link to={`/product/${props.product.id}`}>
                    <Card>
                        <img
                            className="Card-Image"
                            src={getProductImageUrl(props.product.id)}
                            alt={props.product.name}
                        />
                    </Card>

                </Link>
                <CardBody>
                    <CardTitle tag="h5">
                        {props.product.name}
                    </CardTitle>
                    <CardSubtitle tag="b">
                        {props.product.brand}
                    </CardSubtitle>
                    <CardText>
                        {props.product.gender} {props.product.type}
                    </CardText>
                    <CardText tag="b">
                        {currTRY.format(props.product.price)}
                    </CardText>
                </CardBody>
                <div className="btn-group">
                        <Button size="sm" color="primary" onClick={handleAddToBasket}>
                                Add To Card
                            </Button>
                </div>

            </Card>
        </Col>
    );
}
