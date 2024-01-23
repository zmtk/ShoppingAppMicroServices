import React, { useState } from "react";
import { useSelector } from "react-redux";
import { selectCurrentRoles, selectCurrentToken } from "../features/auth/authSlice";
import { Link } from "react-router-dom";
import { Collapse, Nav, NavItem, NavLink, Navbar, NavbarBrand, NavbarToggler, Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from "reactstrap";
import Logout from "../features/auth/Logout";

const NavBar = () => {
    const token = useSelector(selectCurrentToken);
    const roles = useSelector(selectCurrentRoles);
    const [isOpen, setIsOpen] = useState(false);
    const [dropdownOpen, setDropdownOpen] = useState(false);

    const toggle = () => setIsOpen(!isOpen);
    const toggleDropdown = () => setDropdownOpen(!dropdownOpen);

    let adminControls = roles?.find((role) => role === "admin") ? (
        <>
            <DropdownItem tag={Link} to="/userslist">
                UserList
            </DropdownItem>
        </>
    ) : (
        <></>
    );

    let editorControls = roles?.find((role) => role === "editor" || role === "admin") ? (
        <>

            <DropdownItem tag={Link} to="/store">
                Store
            </DropdownItem>

        </>
    ) : (
        <></>
    );
    let userControls = token ? (
        <>
            <Dropdown nav isOpen={dropdownOpen} toggle={toggleDropdown}>
                <DropdownToggle nav caret>
                    User
                </DropdownToggle>
                <DropdownMenu>
                    <DropdownItem tag={Link} to="/dashboard/userinfo">
                        Profile
                    </DropdownItem>
                    <DropdownItem tag={Link} to="/dashboard/addresses">
                        Addresses
                    </DropdownItem>
                    <DropdownItem divider />
                    {editorControls}
                    {adminControls}
                    <DropdownItem divider />
                    <Logout />
                </DropdownMenu>
            </Dropdown>
        </>
    ) : (
        <>
            <NavItem>
                <NavLink tag={Link} to="/login">
                    Login
                </NavLink>
            </NavItem>
            <NavItem>
                <NavLink tag={Link} to="/register">
                    Register
                </NavLink>
            </NavItem>
        </>
    );



    let navBar = (
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-4" container>
            <NavbarBrand tag={Link} exact="true" to="/">
                ShoppingApp
            </NavbarBrand>
            <NavbarToggler onClick={toggle} />
            <Collapse isOpen={isOpen} navbar>
                <Nav className="me-auto" navbar>
                    <NavItem>
                        <NavLink tag={Link} to="/catalog">
                            Catalog
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink tag={Link} to="/basket">
                            Basket
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink tag={Link} to="/orders">
                            Orders
                        </NavLink>
                    </NavItem>
                </Nav>
                <Nav>{userControls}</Nav>
            </Collapse>
        </Navbar>
    );

    return navBar;
};

export default NavBar;
