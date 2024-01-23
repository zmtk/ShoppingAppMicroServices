import { Outlet } from "react-router-dom"
import 'bootstrap/dist/css/bootstrap.min.css';
import NavBar from './NavBar';
import { Container } from "reactstrap";
import { Slide, ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';

const Layout = () => {

    return (
        <>
            <NavBar />
            <Container tag="main" className="layout-container">
                <Outlet />
                
                <ToastContainer
                    position="bottom-right"
                    autoClose={5000}
                    hideProgressBar={false}
                    newestOnTop={false}
                    closeOnClick
                    rtl={false}
                    pauseOnFocusLoss
                    draggable
                    pauseOnHover
                    theme="light"
                    transition={Slide}
                />
            </Container>
        </>


    )
}

export default Layout