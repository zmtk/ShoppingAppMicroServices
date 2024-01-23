import { Col, Row } from "reactstrap"

export const OrderAddressDetail = ({ address }) => {
    return (
        <Col className="mt-5">
            <Row><h4>Delivery Address</h4></Row>
            <Row><strong>{address.addressType}</strong></Row>
            <Row><span>{address.neighborhood} {address.streetAddress}</span></Row>
            <Row><span>{address.neighborhood} / {address.district} / {address.city}</span></Row>
            <Row><span>{address.firstName} {address.lastName} - {address.phoneNumber}</span></Row>
        </Col>
    )
}