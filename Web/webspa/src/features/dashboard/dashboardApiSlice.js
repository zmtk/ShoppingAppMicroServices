import { apiSlice } from "../../app/api/apiSlice";

export const dashboardApiSlice = apiSlice.injectEndpoints({
    endpoints: builder => ({
        getUser: builder.query({
            query: () => '/api/identity/auth/user'
        }),
        getAddresses: builder.query({
            query: () => '/api/identity/address'
        }),
        updateUser: builder.mutation({
            query: userdata => ({
                url: '/api/identity/auth/updateuser',
                method: 'POST',
                body: { ...userdata }
            })
        }),
        addAddress: builder.mutation({
            query: addressdata => ({
                url: '/api/identity/address',
                method: 'POST',
                body: { ...addressdata }
            })
        }),
        updateAddress: builder.mutation({
            query: addressdata => ({
                url: '/api/identity/address/updateaddress',
                method: 'POST',
                body: { ...addressdata }
            })
        }),
        deleteAddress: builder.mutation({
            query: addressdata => ({
                url: '/api/identity/address/removeaddress',
                method: 'DELETE',
                body: { ...addressdata }
            }),
            invalidatesTags: ['/api/identity/address'],
        })
})
})

export const {
    useGetUserQuery,
    useUpdateUserMutation,
    useGetAddressesQuery,
    useAddAddressMutation,
    useUpdateAddressMutation,
    useDeleteAddressMutation
} = dashboardApiSlice