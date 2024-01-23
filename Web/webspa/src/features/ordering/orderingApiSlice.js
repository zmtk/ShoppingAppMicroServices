import { apiSlice } from "../../app/api/apiSlice";

export const orderingApiSlice = apiSlice.injectEndpoints({
    endpoints: builder => ({
        getOrders: builder.query({
            query: () => '/api/order'
        }),
        createOrder: builder.mutation({
            query: orderinfo => ({
                url: '/api/order',
                method: 'POST',
                body: { ...orderinfo }
            }),
        }),
    })
})

export const {
    useGetOrdersQuery,
    useCreateOrderMutation
} = orderingApiSlice