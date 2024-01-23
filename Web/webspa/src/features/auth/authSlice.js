import { createSlice } from "@reduxjs/toolkit";
import jwt_decode from "jwt-decode"

const authSlice = createSlice({
    name: 'auth',
    initialState: { token: null, email: null, roles: [] },
    reducers: {
        setCredentials: (state, action) => {
            const { accessToken } = action.payload
            const decoded = accessToken
                ? jwt_decode(accessToken)
                : undefined
            
            state.token = accessToken
            state.email = decoded?.email || ""            
            // state.roles = decoded?.Roles || []
            state.roles = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || []
        },
        logOut: (state, action) => {
            state.token = null
            state.email = null
            state.roles = null
        }
    },
})

export const { setCredentials, logOut } = authSlice.actions

export default authSlice.reducer

export const selectCurrentToken = (state) => state.auth.token
export const selectCurrentUser = (state) => state.auth.email
export const selectCurrentRoles = (state) => state.auth.roles