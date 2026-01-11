import React, { createContext, useContext, useState, useEffect } from 'react';
import toast, { showToast } from '../components/common/toast';

const AuthContext = createContext(undefined);

// const ADMIN_PASSWORD = 'admin@123';
const API_URI = import.meta.env.VITE_API_URI;


export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);

    useEffect(() => {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken)
            return;
        console.log('comming heere');
        const autoLogin = async () => {
            try{
                const ret = await fetch(API_URI+'/auth/refreshToken', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(refreshToken)
                });
                const res = await ret.json();
                if(!ret.ok){
                    console.log('to bhai token lochavalu che');
                    localStorage.removeItem('refreshToken');
                }
                localStorage.setItem('refreshToken', res.data.refreshToken);
                setUser(res.data);
                console.log(res.data)
                showToast.success('Welcome, ' +res.data.firstName, 'Nice to see you again');
            }catch(e){}
        }
        autoLogin();
    }, []);

    const login = async (email, password, isCandidate=false) => {
        const ret = await fetch(isCandidate ? `${API_URI}/auth/candidate-login` :`${API_URI}/auth/login`,{
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
        const ret = await fetch(API_URI+'/auth/admin-login',{
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(password)
        });
        const res = await ret.json();
        if(!ret.ok)
            throw new Error(res.message);
        const adminUser = {
            firstName: 'Administrator',
            jwtToken: res.data,
            roleName: 'Admin'
        }
        setUser(adminUser)
        // console.log(res.data)
    showToast.success('Welcom Admin', 'Logged in Successfully')
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
        localStorage.removeItem('refreshToken');
    };

    const hasRole = (roles) => {
        if (!user) return false;
        return roles.includes(user.roleName);
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
