import { Card, Button } from "reactstrap";
import { getProductThumbnail } from "../../catalog/components/productImage";
import { currTRY } from "../../../helpers/currencyFormatter";
import { useNavigate } from "react-router-dom";

export const OrderProductDetail = ({ items }) => {
    const navigate = useNavigate();
    const handleOrderAgain = async ({ productId }) => {
        try {
            console.log(productId);
            navigate(`/product/${productId}`);        } catch (err) {
            console.log(err);
        }
    };


    return (
        <>
            {items.map((item) => (
                <Card key={item.productId} className="mt-3 shadow-sm bg-white rounded">
                    <div className="d-flex justify-content-between p-2">

                        <div className="d-flex" style={{ minWidth: '50%' }}>
                            <div className="p-2">
                                <img key={item.productId}
                                    //  className="product-thumb" 
                                    className="rounded border border-2 border-secondary"
                                    alt={item.name}
                                    src={getProductThumbnail(item.productId)}
                                />
                            </div>
                            <div className="p-2">
                                <div className="d-flex flex-column ">
                                    <div className="p-2">{item.brand} {item.name}</div>
                                    <div className="p-2">{currTRY.format(item.price)} x {item.quantity} </div>
                                </div>
                            </div>
                        </div>

                        <div className="d-flex  align-items-center">
                            <h5>{currTRY.format(item.total)}</h5>
                        </div>

                        <div className="d-flex  align-items-center">
                            <Button onClick={() => handleOrderAgain({ productId: item.productId })} color="success">
                                Buy Again
                            </Button>
                        </div>

                    </div>
                </Card>
            ))}
        </>
    );
}