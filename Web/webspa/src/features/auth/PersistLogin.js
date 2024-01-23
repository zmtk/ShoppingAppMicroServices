import { Outlet } from "react-router-dom";
import { useEffect, useRef, useState } from "react";
import { useRefreshMutation } from "./authApiSlice";
import usePersist from "../../hooks/useAuthPersist";
import { useSelector } from "react-redux";
import { selectCurrentToken } from "./authSlice";

const PersistLogin = () => {
    
    const [persist] = usePersist()
    const token = useSelector(selectCurrentToken)
    const effectRan = useRef(false)

    const [trueSuccess, setTrueSuccess] = useState(false)
    
    const [refresh, {
        isUninitialized,
        isLoading,
        isSuccess,
        isError
    }] = useRefreshMutation()

    useEffect(() => {
        if(effectRan.current === true || process.env.NODE_ENV !== 'development'){
            const verifyRefreshToken = async () => {
                console.log('verifying refresh token')
                try {
                    await refresh()
                    setTrueSuccess(true)
                }
                catch(err){
                    console.error(err)
                }
            }
            if(!token && persist) 
            {
                verifyRefreshToken();
            }
        }
        
        return () => effectRan.current = true
    
        // eslint-disable-next-line
    }, [])

    let content
    if(!persist) { // no persist
        content = <Outlet />
    } else if (isLoading) { // persist yes, no token
        content = <p>Loading...</p>
    } else if (isError) { // persist yes, no token
        content = <Outlet /> //<Navigate to="/login"/>
    } else if (isSuccess && trueSuccess) { // persist yes, token yes,
        content = <Outlet />
    } else if (token && isUninitialized) { // persist yes, token yes
        content = <Outlet />
    }
    
    return content;
}
 
export default PersistLogin;
