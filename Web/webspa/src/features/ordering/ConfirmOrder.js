import React, { useCallback, useEffect, useState } from "react";
import Basket from "../basket/Basket";
import { Button, Row, Col, Input } from "reactstrap";
import { useGetAddressesQuery } from "../dashboard/dashboardApiSlice";
import AddressView from "../dashboard/AddressComponents/AddressView";
import { useCreateOrderMutation, useGetOrdersQuery } from "./orderingApiSlice";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

const ConfirmOrder = () => {
    const navigate = useNavigate();
    const [createOrderMutation] = useCreateOrderMutation();
    const { refetch: refetchOrders } = useGetOrdersQuery();

    const handleConfirmOrder = async () => {        
        try {
            
            const response = await createOrderMutation({
                addressId: selectedAddress.id
            }).unwrap();
            
            await refetchOrders();
            toast.success(
                `Your order is created successfully, OrderId: ${response.id}`,
                {
                    toastId: 'ordercreated',
                    position: "bottom-right",
                    theme: "colored",
                }
            );
            navigate('/orders');
        } catch (err) {
            console.error(err);
        }
    };
    
    const {
        data: addressdata,
        isLoading,
        isSuccess
    } = useGetAddressesQuery()

    const [selectedAddress, setSelectedAddress] = useState(null);

    const handleAddressSelection = (event) => {
        const selectedType = event.target.value;

        const foundAdress = addressdata.find(address => address.addressType === selectedType);
        setSelectedAddress(foundAdress);
    };
    
    const selectDefaultAdress = useCallback(() => {
        if (!selectedAddress) {
            if (!addressdata || addressdata.length === 0) {
                setSelectedAddress(null); // or set to a default value as needed
            } else {
                setSelectedAddress(addressdata[0]);
            }
        }
    }, [selectedAddress, addressdata]);

    useEffect(() => {
        selectDefaultAdress();
    }, [selectedAddress, addressdata, selectDefaultAdress]);
   
    let content;

    if (isLoading) {
        content = <div>Loading...</div>
    } else if (isSuccess) {
        content = (
            <>
                <Row>
                    <Col>
                        <Button block onClick={handleConfirmOrder} color="success">CONFIRM ORDER</Button>
                    </Col>
                </Row>
                <Row className="mt-4">
                    <Input
                        id="exampleSelect"
                        name="select"
                        type="select"
                        value={selectedAddress ? selectedAddress.addressType : ""}
                        onChange={handleAddressSelection}
                    >
                        {addressdata.map(address =>
                        (<option key={address.id} value={address.addressType}>
                            {address.addressType}
                        </option>
                        ))}
                    </Input>
                    {selectedAddress &&
                        (
                            <AddressView selectedAddress={selectedAddress} />
                        )}
                </Row>
                <Row className="mt-4">
                    <Basket summary={true} />
                </Row>
            </>
        )
    }

    return content;
};

export default ConfirmOrder;
