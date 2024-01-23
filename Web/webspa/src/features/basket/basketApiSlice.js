import { apiSlice } from "../../app/api/apiSlice";

export const basketApiSlice = apiSlice.injectEndpoints({
    endpoints: builder => ({
        getBasket: builder.query({
            query: () => '/api/basket',
        }),
        setProductQuantity: builder.mutation({
            query: productinfo => ({
                url: '/api/basket',
                method: 'PUT',
                body: { ...productinfo }
            }),
            invalidatesTags: ['/api/basket'],
        }),
        deleteProduct: builder.mutation({
            query: (id) => ({
                url: `/api/basket/${id}`,
                method: 'DELETE',
            }),
            invalidatesTags: ['/api/basket'],
        }),
        emptyBasket: builder.mutation({
            query: () => ({
                url: `/api/basket`,
                method: 'DELETE',
            }),
            invalidatesTags: ['/api/basket'],
        }),
    })
})

export const {
    useGetBasketQuery,
    useSetProductQuantityMutation,
    useDeleteProductMutation,
    useEmptyBasketMutation
} = basketApiSlice