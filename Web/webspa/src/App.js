import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout';
import Public from './components/Public';

import Login from './features/auth/Login';
import Welcome from './features/auth/Welcome';
import RequireAuth from './features/auth/RequireAuth';
import PersistLogin from './features/auth/PersistLogin';

import './App.css';
import UsersList from './features/users/UsersList';
import Register from './features/auth/Register';
import Catalog from './features/catalog/Catalog';
import Product from './features/catalog/Product';
import Basket from './features/basket/Basket';

import DashboardLayout from './features/dashboard/components/DashboardLayout';
import Dashboard from './features/dashboard/Dashboard';
import Userinfo from './features/dashboard/Userinfo';
import Addresses from './features/dashboard/Addresses';
import Orders from './features/ordering/Orders';
import ConfirmOrder from './features/ordering/ConfirmOrder';
import Store from './features/store/Store';
import StoreManagementLayout from './features/store/components/StoreManagementLayout';

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>



        {/* Public Routes */}

        <Route index element={<Public />} />
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route path="catalog" element={<Catalog />} />        <Route path="product/:productId" element={<Product />} />

        <Route path="basket" element={<Basket />} />

        <Route element={<PersistLogin />}>
          {/* Protected Routes */}
          <Route element={<RequireAuth allowedRoles={['user']} />}>
            <Route path="welcome" element={<Welcome />} />
            <Route path="orders" element={<Orders />} />
            <Route path="confirmorder" element={<ConfirmOrder />} />
            <Route element={<DashboardLayout />}>
              <Route path='dashboard' element={<Dashboard />} />
              <Route path='dashboard/userinfo' element={<Userinfo />} />
              <Route path='dashboard/addresses' element={<Addresses />} />
            </Route>
          </Route>

          <Route element={<RequireAuth allowedRoles={['editor', 'admin']} />}>
            <Route element={<StoreManagementLayout />} >
              <Route path="store" element={<Store />} />
            </Route>
          </Route>

          <Route element={<RequireAuth allowedRoles={['admin']} />}>
            <Route path="userslist" element={<UsersList />} />
          </Route>


        </Route>
      </Route>
    </Routes >

  );
}

export default App;

