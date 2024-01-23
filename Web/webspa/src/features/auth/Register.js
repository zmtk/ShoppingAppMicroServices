import { useEffect, useRef, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useRegisterMutation } from "./authApiSlice";
import { Form, FormGroup, Label, Input, Button, Row, Col } from "reactstrap";

const Register = () => {
    const emailRef = useRef()
    const errRef = useRef()
    const [email, setEmail] = useState('')
    const [firstName, setFirstName] = useState('')
    const [lastName, setLastName] = useState('')
    const [pwd, setPwd] = useState('')
    const [errMsg, setErrMsg] = useState('')

    const navigate = useNavigate()
    const [register, { isLoading }] = useRegisterMutation()

    useEffect(() => {
        emailRef.current.focus()
    }, [])

    useEffect(() => {
        setErrMsg('')
    }, [email, firstName, lastName, pwd])

    const handleSubmit = async (e) => {
        e.preventDefault()

        try {
            await register({ email, firstName, lastName, password: pwd })
            setEmail('')
            setFirstName('')
            setLastName('')
            setPwd('')
            navigate('/login')
        } catch (err) {
            if (!err?.originalStatus) {
                setErrMsg('No Server Response');
            } else if (err.originalStatus === 409) {
                setErrMsg('User with e-mail address Already Exist')
            } else {
                setErrMsg('Registration Failed.')
                console.error(err)
            }
            errRef.current.focus();
        }
    }

    const handleEmailInput = (e) => setEmail(e.target.value)
    const handleFirstNameInput = (e) => setFirstName(e.target.value)
    const handleLastNameInput = (e) => setLastName(e.target.value)
    const handlePwdInput = (e) => setPwd(e.target.value)

    const content = isLoading ? <h1>Loading...</h1> : (
        <section className="register">
            <p ref={errRef}
                className={errMsg ? "errmsg" : "offscreen"}
                aria-live="assertive"> {errMsg} </p>
            <h1>Register</h1>
            <Form onSubmit={handleSubmit}>
                <FormGroup>
                    <Label htmlFor="email">Email:</Label>
                    <Input
                        type="email"
                        id="email"
                        ref={emailRef}
                        value={email}
                        onChange={handleEmailInput}
                        autoComplete="off"
                        required
                    />
                </FormGroup>
                <Row>
                    <Col md={6}>
                        <FormGroup>
                            <Label htmlFor="FirstName">FirstName:</Label>
                            <Input
                                type="text"
                                id="FirstName"
                                value={firstName}
                                onChange={handleFirstNameInput}
                                autoComplete="off"
                                required
                            />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label htmlFor="LastName">LastName:</Label>
                            <Input
                                type="text"
                                id="LastName"
                                value={lastName}
                                onChange={handleLastNameInput}
                                autoComplete="off"
                                required
                            />
                        </FormGroup>
                    </Col>
                </Row>
                <FormGroup>
                    <Label htmlFor="pwd">Password:</Label>
                    <Input
                        type="password"
                        id="pwd"
                        value={pwd}
                        onChange={handlePwdInput}
                        required
                    />
                </FormGroup>

                <FormGroup>
                    <Button> Register </Button>
                    <Link to="/login"> Already have an account? </Link>
                </FormGroup>
            </Form>
        </section>
    )

    return content
}

export default Register;