import { useLocation, Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";
import { selectCurrentToken, selectCurrentRoles } from "./authSlice";


const RequireAuth = ({ allowedRoles }) => {
    const token = useSelector(selectCurrentToken)
    const roles = useSelector(selectCurrentRoles)
    const location = useLocation()

    return (
        roles?.find(role => allowedRoles.includes(role))
            ? <Outlet />
            : token
                ? <Navigate to="/unauthorized" state={{ from: location }} replace />
                : <Navigate to="/login" state={{ from: location }} replace />
    );
}

export default RequireAuth;