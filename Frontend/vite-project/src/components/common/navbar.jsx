import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { LogOut, User, Bell } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';

export function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-white shadow-md border-b-2 border-green-500">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <Link to="/dashboard" className="flex items-center space-x-2">
              <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-green-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-xl">R</span>
              </div>
              <span className="text-xl font-bold text-gray-800">
                Recruitment Process Managment System
              </span>
            </Link>
          </div>

          <div className="flex items-center space-x-4">
            <button className="p-2 rounded-full hover:bg-green-50 transition-colors relative">
              <Bell className="w-5 h-5 text-gray-600" />
              <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
            </button>

            <div className="flex items-center space-x-3 border-l pl-4">
              <div className="flex items-center space-x-2">
                <div className="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
                  <User className="w-5 h-5 text-green-600" />
                </div>
                <div className="text-sm">
                  <p className="font-medium text-gray-800">
                    {user?.name}
                  </p>
                  <p className="text-xs text-gray-500 capitalize">
                    {user?.role}
                  </p>
                </div>
              </div>

              <button
                onClick={handleLogout}
                className="p-2 rounded-full hover:bg-red-50 transition-colors"
                title="Logout"
              >
                <LogOut className="w-5 h-5 text-red-600" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}
