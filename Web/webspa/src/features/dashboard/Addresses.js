import { useState, useEffect } from "react";
import { useDeleteAddressMutation, useGetAddressesQuery, useUpdateAddressMutation } from "./dashboardApiSlice";
import { Form, Button } from "reactstrap";
import { useCallback } from "react";

import AddAddress from "./AddressComponents/AddAddress";
import Address from "./AddressComponents/Address";
import AddressSelector from "./AddressComponents/AddressSelector";
import { toast } from "react-toastify";

const Addresses = ({ isReadOnly = false }) => {

    const [selectedAddress, setSelectedAddress] = useState(null);
    const [updateAdress] = useUpdateAddressMutation()
    const [deleteAddress] = useDeleteAddressMutation()

    const {
        data: addressdata,
        isLoading,
        isSuccess,
        refetch: refetchAddresses,
    } = useGetAddressesQuery()


    const [addAddressModal, setAddAddressModal] = useState(false);

    const toggleAddAddressModal = () => setAddAddressModal(!addAddressModal);

    const handleAddressSelection = (event) => {
        const selectedType = event.target.value;

        const foundAdress = addressdata.find(address => address.addressType === selectedType);
        setSelectedAddress(foundAdress);
    };

    const handleAddressChange = useCallback(
        (event) => {
            const { name, value } = event.target;
            setSelectedAddress((prevAddress) => ({ ...prevAddress, [name]: value }));
        },
        [setSelectedAddress]
    );


    const handleSaveButton = async () => {
        try {
            await updateAdress({ ...selectedAddress }).unwrap()
            await refetchAddresses();
            toast.success(
                `Address updated successfully`,
                {
                    toastId: 'addressupdated',
                }
            );
        } catch (err) {
            console.log(err.message);
        }
    }

    const selectDefaultAdress = useCallback(() => {
        if (!selectedAddress) {
            if (!addressdata || addressdata.length === 0) {
                setSelectedAddress(null); // or set to a default value as needed
            } else {
                setSelectedAddress(addressdata[0]);
            }
        }
    }, [selectedAddress, addressdata]);

    useEffect(() => {
        selectDefaultAdress();
    }, [selectedAddress, addressdata, selectDefaultAdress]);

    const handleDeleteButton = async () => {
        console.log("handle delete button")
        try {
            const Response = await deleteAddress({ id: selectedAddress.id }).unwrap()
            console.log(Response);

            await refetchAddresses();
            toast.success(
                `Address deleted successfully`,
                {
                    toastId: 'addressdeleted',
                }
            );
            setSelectedAddress(null);
            selectDefaultAdress();

        } catch (err) {
            console.log(err.message);
        }

    }

    let addressInfoForm = (
        <Form>

            <AddAddress
                refetchAddresses={refetchAddresses}
                setSelectedAddress={setSelectedAddress}
                addAddressModal={addAddressModal}
                setAddAddressModal={setAddAddressModal}
                toggleAddAddressModal={toggleAddAddressModal}
            />

            {!addressdata || addressdata.length === 0 ? (
                <div>
                    {/* Render this content when addressdata is not available or is empty */}
                    <p>No addresses available.</p>
                    <Button block onClick={toggleAddAddressModal} color="success"> Add New Address</Button>

                </div>
            ) : (
                <AddressSelector
                    selectedAddress={selectedAddress}
                    handleAddressSelection={handleAddressSelection}
                    addressdata={addressdata}
                    toggleAddAddressModal={toggleAddAddressModal}
                />
            )}

            {selectedAddress && (
                <Address
                    selectedAddress={selectedAddress}
                    handleAddressChange={handleAddressChange}
                    handleSaveButton={handleSaveButton}
                    handleDeleteButton={handleDeleteButton}
                    isReadOnly={isReadOnly}
                />
            )}

        </Form>
    );
    let content;

    if (isLoading) {
        content = <div>Loading...</div>
    } else if (isSuccess) {
        if (addressdata) {
            content = addressInfoForm
        }
    }

    return content;
}

export default Addresses;