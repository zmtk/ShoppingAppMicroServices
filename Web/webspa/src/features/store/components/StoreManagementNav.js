import { Link } from "react-router-dom";
import { ListGroup, ListGroupItem } from "reactstrap";
import { useLocation } from "react-router-dom";
import storeManagementRoutes from "./storeManagementRoutes";

const StoreManagementNav = () => {
    const location = useLocation();
    let path = location.pathname;

    return (
        <section id="usernav">
            <ListGroup>
                <div>
                    {storeManagementRoutes.map((item, index) => (
                        <ListGroupItem
                            key={index}
                            action
                            active={path === item.route}
                            to={item.route}
                            tag={Link}
                        >
                            {item.label}
                        </ListGroupItem>
                    ))}
                </div>
            </ListGroup>
        </section>

    )
}

export default StoreManagementNav