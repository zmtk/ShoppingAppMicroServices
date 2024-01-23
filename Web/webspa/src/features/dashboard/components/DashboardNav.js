import { Link } from "react-router-dom";
import { ListGroup, ListGroupItem } from "reactstrap";
import { useLocation } from "react-router-dom";
import dashboardRoutes from "./DashboardRoutes";

const DashboardNav = () => {
    const location = useLocation();
    let path = location.pathname;

    return (
        <section id="usernav">
            <ListGroup>
                <div>
                    {dashboardRoutes.map((item, index) => (
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

export default DashboardNav