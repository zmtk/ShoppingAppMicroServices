import { Card, Row,Col } from "reactstrap";

const AddressView = ({ selectedAddress }) => {
    return (
        <Card>
            <Col className="m-4">
                <Row><h4>Delivery Address</h4></Row>
                <Row><strong>{selectedAddress.addressType}</strong></Row>
                <Row><span>{selectedAddress.streetAddress}</span></Row>
                <Row><span>{selectedAddress.neighborhood} / {selectedAddress.district} / {selectedAddress.city}</span></Row>
                <Row><span>{selectedAddress.firstName} {selectedAddress.lastName} - {selectedAddress.phoneNumber}</span></Row>
            </Col>
        </Card>
    );
}

export default AddressView;
