import { apiSlice } from "../../app/api/apiSlice";

export const usersApiSlice = apiSlice.injectEndpoints({
    endpoints: builder => ({
        getUsers: builder.query({
            query: () => '/api/identity/auth/users',
            keepUnusedDataFor:5, //Cache Remove Later
        })
    })
})

export const {
    useGetUsersQuery
} = usersApiSlice