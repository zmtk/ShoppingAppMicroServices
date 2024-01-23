import { Outlet } from "react-router-dom"
import 'bootstrap/dist/css/bootstrap.min.css';
import { Col, Container, Row } from "reactstrap";
import DashboardNav from './DashboardNav';

const DashboardLayout = () => {


    return (
        <>
            <Container>
                <Row>
                    <Col xs="4" lg="2">
                        < DashboardNav />
                    </Col>
                    <Col>
                        <Outlet />
                    </Col>
                </Row>
            </Container>
        </>
    )



}

export default DashboardLayout