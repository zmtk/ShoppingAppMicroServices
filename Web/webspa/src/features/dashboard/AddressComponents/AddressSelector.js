import { FormGroup,Label,Row,Col,Input,Button } from "reactstrap";

const AddressSelector = ({selectedAddress, handleAddressSelection, addressdata, toggleAddAddressModal}) => {
    return(
        <FormGroup>
                <Label for="exampleSelect">
                    Select
                </Label>
                <Row>
                    <Col lg={10} md={8} sm={6}>
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
                    </Col>
                    <Col lg={2} md={4} sm={6}>
                        <Button block onClick={toggleAddAddressModal} color="success"> New Address</Button>
                    </Col>
                </Row>
            </FormGroup>
    )
}

export default AddressSelector;