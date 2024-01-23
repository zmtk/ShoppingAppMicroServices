import { Outlet } from "react-router-dom"
import 'bootstrap/dist/css/bootstrap.min.css';
import { Col, Container, Row } from "reactstrap";
// import StoreManagementNav from "./StoreManagementNav";

const StoreManagementLayout = () => {


    return (
        <>
            <Container>
                <Row>
                    {/* <Col xs="4" lg="2">
                        < StoreManagementNav />
                    </Col> */}
                    <Col>
                        <Outlet />
                    </Col>
                </Row>
            </Container>
        </>
    )

}

export default StoreManagementLayout