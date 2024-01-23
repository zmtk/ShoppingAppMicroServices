import { AccordionItem, AccordionHeader, AccordionBody, Col, Row } from "reactstrap";
import { currTRY } from "../../../helpers/currencyFormatter";
import { formatDate, formatTime } from "../../../helpers/dateFormatter"
import { getOrderProductThumbnail } from "../../catalog/components/productImage";
import { OrderProductDetail } from "./OrderProductDetail";
import { OrderAddressDetail } from "./OrderAddressDetail";

const OrderThumbnail = ({ items }) => {
    return (
        <div style={{ display: 'flex', justifyContent: 'center' }}>
            {items.map((item) => (
                <img key={item.productId}
                    className="rounded-circle img-thumbnail border-0"
                    style={{
                        padding: '2px',
                        marginLeft: item.productId > 0 ? '-20px' : '0',
                    }}
                    alt={item.name}
                    src={getOrderProductThumbnail(item.productId)}
                />
            ))}
        </div>
    )
}

export const Order = ({ order }) => {
    return (
        <AccordionItem>
            <AccordionHeader targetId={order.id.toString()}>
                <Col>
                    <OrderThumbnail items={order.basketItems.slice(0, 3)} />
                </Col>
                <Col >
                    <Row>

                        <div>
                            Order Number: <strong>{order.id}</strong>
                        </div>
                    </Row>
                    <Row>
                        <div>
                            <small className="text-muted">
        
                                <strong>{formatDate(order.date)}</strong>
                                <span className="ms-1">{/* Add margin start */}
                                    {formatTime(order.date)}
                                </span>
                            </small>
                        </div>
                    </Row>
                </Col>

                <Col>{order.orderStatus}</Col>

                <Col>
                    {currTRY.format(order.totalPrice)}

                </Col>
            </AccordionHeader>


            <AccordionBody accordionId={order.id.toString()} className="shadow-lg bg-white rounded">
                <Col className="p-3">
                    <Row >
                        <OrderProductDetail items={order.basketItems} />

                    </Row>
                    <Row>
                        <OrderAddressDetail address={order.deliveryAddress} />
                    </Row>
                </Col>
            </AccordionBody>

        </AccordionItem>
    );
}