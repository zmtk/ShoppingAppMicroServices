import { Form, Row, Col, FormGroup, Label, Input, Button } from "reactstrap";
import { useState,useEffect } from "react";
import { useGetUserQuery, useUpdateUserMutation } from "./dashboardApiSlice";
import { toast } from "react-toastify";

const Userinfo = () => {

    const [updateUser] = useUpdateUserMutation()

    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [dateOfBirth, setDateOfBirth] = useState("");
    
    const {
        data: userdata,
        isLoading,
        isSuccess,
        refetch: refetchUser,
    } = useGetUserQuery()

    useEffect(() => {
        if (isSuccess) {
            setFirstName(userdata.firstName);
            setLastName(userdata.lastName);
            setEmail(userdata.email);
            setPhoneNumber(userdata.phoneNumber);
            setDateOfBirth(userdata.dateOfBirth);
        }
    }, [isSuccess, userdata]);


    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            await updateUser(
                {
                    firstName, lastName, email, phoneNumber, dateOfBirth
                }).unwrap()
            await refetchUser();
            toast.success(
                `User info updated successfully`,
                {
                    toastId: 'userupdated',
                }
            );
        } catch (err) {
            console.log(err.message)
        }

    }

    if (isLoading) {
        return <div>Loading...</div>
    } else if (isSuccess) {

        let content = (
            <section>
                <Form onSubmit={handleSubmit}>
                    <Row>
                        <Col md={6}>
                            <FormGroup>
                                <Label for="firstName">
                                    FirstName
                                </Label>
                                <Input
                                    id="firstName"
                                    name="firstName"
                                    placeholder="firstName"
                                    onChange={(e) => setFirstName(e.target.value)}
                                    value={firstName}
                                    type="text"
                                />
                            </FormGroup>
                        </Col>
                        <Col md={6}>
                            <FormGroup>
                                <Label for="LastName">
                                    LastName
                                </Label>
                                <Input
                                    id="LastName"
                                    name="LastName"
                                    placeholder="LastName"
                                    onChange={(e) => setLastName(e.target.value)}
                                    value={lastName}
                                    type="text"
                                />
                            </FormGroup>
                        </Col>
                    </Row>
                    <FormGroup>
                        <Label for="emailAddress">
                            Email Adress
                        </Label>
                        <Input
                            id="emailAdress"
                            name="emailAdress"
                            placeholder="email@example.com"
                            onChange={(e) => setEmail(e.target.value)}
                            value={email}
                            type="email"
                        />
                    </FormGroup>
                    <FormGroup>
                        <Label for="phoneNumber">
                            Phone Number
                        </Label>
                        <Input
                            id="phoneNumber"
                            name="number"
                            placeholder="number placeholder"
                            onChange={(e) => setPhoneNumber(e.target.value)}
                            value={phoneNumber}
                            type="numeric"
                        />
                    </FormGroup>
                    <FormGroup>
                        <Label for="exampleDate">
                            Date of birth
                        </Label>
                        <Input
                            id="exampleDate"
                            name="date"
                            placeholder="date placeholder"
                            onChange={(e) => setDateOfBirth(e.target.value)}
                            value={dateOfBirth}
                            type="date"
                        />
                    </FormGroup>

                    <Button color="primary">
                        Update
                    </Button>
                </Form>
            </section>
        )

        return content;

    }



}

export default Userinfo;