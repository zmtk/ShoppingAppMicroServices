import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { setCredentials, logOut } from '../../features/auth/authSlice'

const baseQuery = fetchBaseQuery({
    // baseUrl: 'http://localhost:5199', //local identityapi
    // baseUrl: 'http://localhost:5220', //local catalogapi
    baseUrl: 'http://shoppingapp.com',
    credentials: 'include',
    prepareHeaders: (headers, { getState }) => {
        const token = getState().auth.token
        
        if (token) { //add Bearer
            headers.set("Authorization", `Bearer ${token}`)
        }
        return headers
    }
})

const baseQueryWithReauth = async (args, api, extraOptions) => {
    let result = await baseQuery(args, api, extraOptions)
    if (result?.error?.status === 401) {
        // refresh access token
        const refreshResult = await baseQuery(
            '/api/identity/auth/refresh', api, extraOptions)
        if (refreshResult?.data) {
            // store the new token
            api.dispatch(setCredentials({ ...refreshResult.data }))

            //use new access token with original query
            result = await baseQuery(args, api, extraOptions)
        } else {
            api.dispatch(logOut())
        }
    }

    return result

}

export const apiSlice = createApi({
    baseQuery: baseQueryWithReauth,
    endpoints: builder => ({})
})