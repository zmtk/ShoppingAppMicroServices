import { Accordion } from "reactstrap";
import { useGetOrdersQuery } from "./orderingApiSlice";
import { useState } from "react";
import { Order } from "./components/Order";

const Orders = () => {
    const {
        // Provide a default value if data is undefined
        data: { Orders: orders } = { Orders: [] },
        isLoading,
        isSuccess,
        isError,
        error
    } = useGetOrdersQuery();

    let content;
    // const [open, setOpen] = useState(orders.length);
    const [open, setOpen] = useState("-1");
    const toggle = (id) => {
        console.log(orders.length)
        if (open === id) {
            setOpen();
        } else {
            setOpen(id);
        }
    };


    if (isLoading) {
        content = <p>Loading...</p>
    } else if (isSuccess) {
        content = (
            <>
                <Accordion open={open} toggle={toggle} className="p-3" >
                    {orders.slice().reverse().map((order) => (
                            <Order className="mt-4" order={order} key={order.id}/>
                    ))}
                </Accordion>
            </>
        );
    } else if (isError) {
        console.log(error);
    }
    return content;
}

export default Orders;
