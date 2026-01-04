import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Input } from '../components/common/input';
import { Lock, Mail, User as UserIcon } from 'lucide-react';
import { Button } from '../components/common/button';
import { showToast } from '../components/common/toast';
import { useAuth } from '../context/AuthContext';

export function CandidateLogin({verifiedEmail}) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  useEffect(()=>{
    if(verifiedEmail) setEmail(verifiedEmail);
  },[verifiedEmail]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(email, password, true);
      showToast.success('Welcome!', 'You have successfully logged in');
      navigate('/dashboard');
    } catch (err) {
      setError('Invalid email or password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-blue-100 to-blue-200 flex items-center justify-center px-4">
      <div className="max-w-md w-full">
        <div className="text-center mb-8">
          <div className="inline-block w-20 h-20 bg-gradient-to-br from-blue-400 to-blue-600 rounded-2xl flex content-center justify-items-center mb-4 shadow-lg">
            <UserIcon className="w-15 h-15 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-gray-800 mb-2">Welcome Candidate</h1>
          <p className="text-gray-600">Sign in to your RecruitPro candidate account</p>
        </div>

        <div className="bg-white rounded-2xl shadow-2xl p-8">
          <form onSubmit={handleSubmit} className="space-y-6">
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
                {error}
              </div>
            )}

            <div>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                <input
                  type="email"
                  placeholder="Email address"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="w-full pl-12 pr-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>

            <div>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                <input
                  type="password"
                  placeholder="Password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="w-full pl-12 pr-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>

            <Button type="submit" className="w-full" variant="success" size="lg" disabled={loading}>
              {loading ? 'Signing in...' : 'Sign In'}
            </Button>
          </form>

          <div className="mt-6 text-center space-y-4">
            {/* <p className="text-gray-600">
              Don't have an account?{' '}
              <Link to="/signup" className="text-blue-600 font-semibold hover:text-blue-700">
              </Link>
              </p> */}
              <p className="text-gray-600">
                    Please Check your eMail for Credential
              </p>
          </div>
        </div>
      </div>
    </div>
  );
}
