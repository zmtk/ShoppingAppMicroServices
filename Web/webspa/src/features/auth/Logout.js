import { Link, useNavigate } from "react-router-dom";
import { useLogoutMutation } from "./authApiSlice";
import { useEffect } from "react";
import { DropdownItem } from "reactstrap";

const Logout = () => {

    const navigate = useNavigate()

    const [logout, {
        isLoading,
        isSuccess,
        isError,
        error
    }] = useLogoutMutation()

    useEffect(() => {
        if (isSuccess) navigate('/')
    }, [isSuccess, navigate])

    if (isLoading) return <p>Logging out...</p>
    if (isError) return <p>Error: {error.data?.message}</p>
    return (
        <DropdownItem tag={Link} onClick={logout}>
            Logout
        </DropdownItem>
    )
}

export default Logout;