import React, { useEffect, useCallback, useState } from "react";
import { useEmptyBasketMutation, useGetBasketQuery } from "./basketApiSlice";
import { BasketItem } from "./BasketItem";
import { Button, Row, Col } from "reactstrap";
import { currTRY } from "../../helpers/currencyFormatter";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

const Basket = ({ summary }) => {
    const navigate = useNavigate();

    const {
        refetch: refetchBasket,
        data: basket,
        isLoading,
        isSuccess,
        isError,
        error,
    } = useGetBasketQuery();
    
    const [totalPrice, setTotalPrice] = useState(() => {
        return basket?.basketTotal || -1;
    });

    useEffect(() => {
        if ((isError && error?.status === 404) || totalPrice === -1) {            toast.error(
                "There's nothing in your basket", {toastId:"emptybasket"}
            );
            navigate("/catalog");
        }
    }, [isError, error, navigate, totalPrice]);

    const [emptyBasketMutation] = useEmptyBasketMutation();

    const emptyBasket = useCallback(async () => {
        try {
            await emptyBasketMutation().unwrap();
            toast.success(
                "The basket has been successfully emptied."
            );
            await refetchBasket();

        } catch (err) {
            console.log(err);
        }
    }, [emptyBasketMutation, refetchBasket]);

   

    useEffect(() => {
        // Only run the effect on initial mount
        if (isSuccess) {
            setTotalPrice(basket?.basketTotal || -1);

            const inactiveProductNames = basket?.basketItems
                .filter((product) => product.inactive)
                .map((product) => product.name);

            if (inactiveProductNames.length > 0) {
                inactiveProductNames.forEach((productName) => {
                    toast.warn(
                        `${productName} is out of stock and removed from the basket`,
                        {
                            toastId: 'outofstockproduct',
                            position: "bottom-center",

                        }
                    );
                });
            }
        }
    }, [isSuccess, basket]);


    let content;

    if (isLoading) {
        content = <p>Loading...</p>;
    } else if (isSuccess) {
        const items = basket.basketItems.map((product) => {
            // Check if it's in summary mode and the product is not inactive
            if (summary && product.inactive) {
                return null; // Skip rendering for inactive items in summary mode
            }

            return (
                <BasketItem
                    key={product.productId}
                    product={product}
                    setBasketTotal={setTotalPrice}
                    refetchBasket={refetchBasket}
                />
            );
        });


        content = (
            <>
                <Row className="mb-4">
                    <Col lg={8} md={8}>
                        {basket.basketItems.filter((item) => !item.inactive).length} items { }
                    </Col>
                    <Col lg={1} md={1}>{currTRY.format(totalPrice)}</Col>
                    <Col lg={3} md={3}>
                        <Row>
                            <Col>
                                <Button onClick={emptyBasket} block size="sm" color="danger">
                                    Remove All
                                </Button>
                            </Col>
                            <Col>
                                <Link to="/confirmorder">
                                    <Button block size="sm" color="success">
                                        CheckOut
                                    </Button>
                                </Link>
                            </Col>
                        </Row>
                    </Col>
                </Row>

                <Row tag="h6">
                    <Col lg={5} md={5}>
                        <Row>
                            <Col lg={4} md={5}> Product </Col>
                            <Col lg={8} md={7}> Description </Col>
                        </Row>
                    </Col>
                    <Col md={1}>Price</Col>
                    <Col md={2} className="text-center">
                        Quantity
                    </Col>
                    <Col lg={1} md={1}>Total</Col>
                    <Col lg={3} md={3}>Actions</Col>
                </Row>

                <hr className="my-2" />

                {items}
            </>
        );
    }

    return content;
};

export default Basket;
