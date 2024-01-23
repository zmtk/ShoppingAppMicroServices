import { useGetUsersQuery } from "./usersApiSlice";
import { Link } from "react-router-dom";

const UsersList = () => {
    const {
        data: users,
        isLoading,
        isSuccess,
        isError,
        error
    } = useGetUsersQuery()

    let content;
    
    if (isLoading) {
        content = <p>Loading...</p>;
    } else if (isSuccess) {
        content = (
            <section className="users">
                <h1>Users List</h1>
                <ul>
                    {/* Dont use the iterator for map for lists or it can cause problems  */}
                    {/* fix how? */}
                    {users.map((user, i) =>{
                        return <li key={i}>{user.email}</li>
                    })}
                </ul>
                <Link to="/welcome">Back to Welcome</Link>
            </section>
        )        
    } else if (isError) {
        content = <p>{JSON.stringify(error)}</p>;
    }
    return content
}

export default UsersList