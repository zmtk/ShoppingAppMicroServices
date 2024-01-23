import { Col, Row, FormGroup, Button } from "reactstrap";
import AddressInput from "./AddressInput";

const Address = ({ selectedAddress, handleAddressChange, handleSaveButton, handleDeleteButton, isReadOnly }) => {

    let content = (
        <>
            <AddressInput
                label="Address Type"
                name="addressType"
                value={selectedAddress.addressType}
                onChange={handleAddressChange}
            />

            <Row>
                <Col md={6}>
                    <AddressInput
                        label="First Name"
                        name="firstName"
                        value={selectedAddress.firstName}
                        onChange={handleAddressChange}
                    />
                </Col>
                <Col md={6}>
                    <AddressInput
                        label="Last Name"
                        name="lastName"
                        value={selectedAddress.lastName}
                        onChange={handleAddressChange}
                    />
                </Col>
            </Row>
            <Row>
                <Col md={6}>
                    <AddressInput
                        label="Phone Number"
                        name="phoneNumber"
                        value={selectedAddress.phoneNumber}
                        onChange={handleAddressChange}
                    />
                </Col>
                <Col md={6}>
                    <AddressInput
                        name="city"
                        value={selectedAddress.city}
                        onChange={handleAddressChange}
                    />
                </Col>
            </Row>
            <Row>
                <Col md={6}>
                    <AddressInput
                        name="district"
                        value={selectedAddress.district}
                        onChange={handleAddressChange}
                    />
                </Col>
                <Col md={6}>
                    <AddressInput
                        name="neighborhood"
                        value={selectedAddress.neighborhood}
                        onChange={handleAddressChange}
                    />
                </Col>
            </Row>

            <AddressInput
                name="streetAddress"
                value={selectedAddress.streetAddress}
                type="textarea"
                onChange={handleAddressChange}
            />

            <FormGroup className="d-flex justify-content-between">
                <Row>
                    <Col>
                        <Button onClick={handleSaveButton} color="success">Save</Button>
                    </Col>
                    <Col>
                        <Button onClick={handleDeleteButton} color="danger">Delete</Button>
                    </Col>
                </Row>
            </FormGroup>


        </>
    )


    return content;
}

export default Address;