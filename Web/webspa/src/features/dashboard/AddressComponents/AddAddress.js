import { useCallback, useState } from "react";
import { useAddAddressMutation } from "../dashboardApiSlice";
import AddressInput from "./AddressInput";

import { Row, Col, Modal, ModalHeader, ModalBody, ModalFooter, Button  } from "reactstrap";
import { toast } from "react-toastify";

const AddAddress = ({refetchAddresses, setSelectedAddress, addAddressModal, setAddAddressModal, toggleAddAddressModal}) => {

    const [addAddress] = useAddAddressMutation()

    const [newAddress, setNewAddress] = useState({
        addressType: '', firstName: '', lastName: '',
        phoneNumber: '', city: '', district: '',
        neighborhood: '', streetAddress: ''
    });

    const handleNewAddress = useCallback(
        (event) => {
            const { name, value } = event.target;
            setNewAddress((prevAddress) => ({ ...prevAddress, [name]: value }));
        },
        [setNewAddress]
    );

    const handleAddNewAddress = async () => {
        try {
            const Response = await addAddress({ ...newAddress }).unwrap()

            await refetchAddresses();

            setSelectedAddress(newAddress);
            setAddAddressModal(false);
            toast.success(
                `New Address added successfully`,
                {
                    toastId: 'addresscreated',
                }
            );
            console.log(Response);
        } catch (err) {
            console.log(err.message);
        }
    }

    return (
        <Modal isOpen={addAddressModal} toggle={toggleAddAddressModal}>
            <ModalHeader toggle={toggleAddAddressModal}>Modal title</ModalHeader>
            <ModalBody>
                <>
                    <AddressInput
                        label="Address Type"
                        name="addressType"
                        onChange={handleNewAddress}
                    />

                    <Row>
                        <Col md={6}>
                            <AddressInput
                                label="First Name"
                                name="firstName"
                                onChange={handleNewAddress}
                            />
                        </Col>
                        <Col md={6}>
                            <AddressInput
                                label="Last Name"
                                name="lastName"
                                onChange={handleNewAddress}
                            />
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <AddressInput
                                label="Phone Number"
                                name="phoneNumber"
                                onChange={handleNewAddress}
                            />
                        </Col>
                        <Col md={6}>
                            <AddressInput
                                name="city"
                                onChange={handleNewAddress}
                            />
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <AddressInput
                                name="district"
                                onChange={handleNewAddress}
                            />
                        </Col>
                        <Col md={6}>
                            <AddressInput
                                name="neighborhood"
                                onChange={handleNewAddress}
                            />
                        </Col>
                    </Row>

                    <AddressInput
                        name="streetAddress"
                        type="textarea"
                        onChange={handleNewAddress}
                    />
                </>
            </ModalBody>
            <ModalFooter>
                <Button color="primary" onClick={handleAddNewAddress}>
                    Add New Address
                </Button>{' '}
                <Button color="secondary" onClick={toggleAddAddressModal}>
                    Cancel
                </Button>
            </ModalFooter>
        </Modal>
    )
}

export default AddAddress;