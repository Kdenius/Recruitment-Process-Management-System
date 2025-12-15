import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Login } from './login';
import toast from 'react-hot-toast';

function Verify() {
  const { token, email } = useParams();
  const [verifiedEmail, setVerifiedEmail] = useState('');  // Correct state initialization

  useEffect(() => {
    const verifyEmail = async () => {
      try {
        const res = await fetch(`${import.meta.env.VITE_API_URI}/auth/verify/${token}/${email}`);
        const data = await res.json();
        if (res.ok) {
          setVerifiedEmail(email);  // Set the verified email to state
          return data.message;
        }
        
        // Throw an error if the response is not OK
        // console.log(data)
        throw new Error(data.message);
      } catch (e) {
        throw new Error(e.message);
      }
    };

    // Trigger the toast promise only once when the component is mounted
    toast.promise(
      verifyEmail(),
      {
        loading: 'Verifying email...',
        success: (mes) => mes ||'verified successfully!',
        error: (err) => err.message || 'Invalid or expired verification link',
      }
    );
  }, [token, email]);  // Add token and email to the dependency array to ensure effect runs when they change

  return (
    <>
      <Login verifiedEmail={verifiedEmail} /> {/* Pass the verified email to Login */}
    </>
  );
}

export default Verify;
