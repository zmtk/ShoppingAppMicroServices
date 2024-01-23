import { FormGroup, Form, Label, Input, Button } from "reactstrap";
import { useRef, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { useDispatch } from "react-redux";
import { setCredentials } from "./authSlice";
import { useLoginMutation } from "./authApiSlice";
import usePersist from "../../hooks/useAuthPersist";

const Login = () => {
    const emailRef = useRef()
    const errRef = useRef()
    const [email, setEmail] = useState('')
    const [pwd, setPwd] = useState('')
    const [errMsg, setErrMsg] = useState('')
    const [persist, setPersist] = usePersist()

    const navigate = useNavigate()

    const [login, { isLoading }] = useLoginMutation()
    const dispatch = useDispatch()

    useEffect(() => {
        emailRef.current.focus()
    }, [])

    useEffect(() => {
        setErrMsg('')
    }, [email, pwd])

    const handleSubmit = async (e) => {
        e.preventDefault()

        try {
            const { accessToken } = await login({ email, password: pwd }).unwrap()
            dispatch(setCredentials({ accessToken }))
            setEmail('')
            setPwd('')
            navigate('/welcome')
        } catch (err) {
            if (!err?.originalStatus) {
                setErrMsg('No Server Response');
            } else if (err.originalStatus === 404) {
                setErrMsg('User not found');
            } else if (err.originalStatus === 401) {
                setErrMsg('Invalid credentials');
            } else {
                setErrMsg('Login Failed.')
                console.error(err)
            }
            errRef.current.focus();
        }
    }

    const handleEmailInput = (e) => setEmail(e.target.value)
    const handlePwdInput = (e) => setPwd(e.target.value)
    const handleToggle = () => setPersist(prev => !prev)

    const content = isLoading ? <h1>Loading...</h1> : (
        <section className="login">
            <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"}
                aria-live="assertive">{errMsg}</p>
            <h1>User Login</h1>
            <Form onSubmit={handleSubmit}>
                <FormGroup>
                    <Label htmlFor="email">Email:</Label>
                    <Input
                        type="email"
                        id="email"
                        ref={emailRef}
                        value={email}
                        onChange={handleEmailInput}
                        required
                    />
                </FormGroup>
                <FormGroup>
                    <Label htmlFor="password">Password:</Label>
                    <Input
                        type="password"
                        id="password"
                        value={pwd}
                        onChange={handlePwdInput}
                        required
                    />
                </FormGroup>
                <FormGroup>
                    <Button>Sign In</Button>

                    <Label htmlFor="persist">
                        Trust this device
                    </Label>
                    <Input
                        type="checkbox"
                        id="persist"
                        onChange={handleToggle}
                        checked={persist}
                    />
                </FormGroup>
            </Form>
        </section>
    )

    return content
}

export default Login;