import React, { createContext, useContext, useState, useEffect } from 'react';
import { showToast } from '../components/common/toast';

const AuthContext = createContext(undefined);

const ADMIN_PASSWORD = 'admin@123';
const API_URI = import.meta.env.VITE_API_URI;


export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);

    useEffect(() => {
        const storedUser = localStorage.getItem('user');
        if (storedUser) {
            setUser(JSON.parse(storedUser));
        }
    }, []);

    const login = async (email, password) => {
        const ret = await fetch(API_URI +'/auth/login',{
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        });
        const res = await ret.json();
        if(!ret.ok)
            throw new Error(res.message);
        setUser(res.data);
        localStorage.setItem('refreshToken', res.data.refreshToken);
        // const userData = {
        //     id: Math.random().toString(36).substr(2, 9),
        //     email,
        //     name: email.split('@')[0],
        //     role: 'viewer',
        //     createdAt: new Date().toISOString(),
        // };

        // setUser(userData);
        // localStorage.setItem('user', JSON.stringify(userData));
    };

    const adminLogin = async (password) => {
        if (password !== ADMIN_PASSWORD) {
            throw new Error('Invalid admin password');
        }

        const adminUser = {
            id: 'admin-1',
            email: 'admin@system.com',
            name: 'System Administrator',
            role: 'admin',
            createdAt: new Date().toISOString(),
        };

        setUser(adminUser);
        localStorage.setItem('user', JSON.stringify(adminUser));
    };

    const signup = async (email, password, fname, lname) => {
        const res = await fetch(API_URI + '/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                firstName: fname,
                lastName: lname,
                email: email,
                password: password,
                clientUrl: `${import.meta.env.VITE_APP_URI}/verify`
            }),
        });
        const data = await res.json();
        if (!res.ok) {
            throw new Error(data.message)
        }
        showToast.success('User Created', data.message)
        // const userData = {
        //   id: Math.random().toString(36).substr(2, 9),
        //   email,
        //   name,
        //   role: 'viewer',
        //   createdAt: new Date().toISOString(),
        // };

        // setUser(userData);
        // localStorage.setItem('user', JSON.stringify(userData));
    };

    const logout = () => {
        setUser(null);
        localStorage.removeItem('user');
    };

    const hasRole = (roles) => {
        if (!user) return false;
        return roles.includes(user.role);
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                login,
                adminLogin,
                signup,
                logout,
                isAuthenticated: !!user,
                hasRole,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}
