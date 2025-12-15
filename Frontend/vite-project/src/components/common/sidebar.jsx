import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import {
  LayoutDashboard,
  Briefcase,
  Users,
  Calendar,
  FileText,
  BarChart3,
  Settings,
  UserCog,
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';

const navItems = [
  {
    name: 'Dashboard',
    path: '/dashboard',
    icon: LayoutDashboard,
    roles: ['admin', 'recruiter', 'hr', 'interviewer', 'reviewer', 'viewer'],
  },
  {
    name: 'Jobs',
    path: '/jobs',
    icon: Briefcase,
    roles: ['admin', 'recruiter', 'hr', 'viewer'],
  },
  {
    name: 'Candidates',
    path: '/candidates',
    icon: Users,
    roles: ['admin', 'recruiter', 'hr', 'reviewer', 'interviewer', 'viewer'],
  },
  {
    name: 'Interviews',
    path: '/interviews',
    icon: Calendar,
    roles: ['admin', 'recruiter', 'hr', 'interviewer', 'viewer'],
  },
  {
    name: 'Documents',
    path: '/documents',
    icon: FileText,
    roles: ['admin', 'hr', 'viewer'],
  },
  {
    name: 'Reports',
    path: '/reports',
    icon: BarChart3,
    roles: ['admin', 'recruiter', 'hr', 'viewer'],
  },
  {
    name: 'Users',
    path: '/users',
    icon: UserCog,
    roles: ['admin'],
  },
  {
    name: 'Settings',
    path: '/settings',
    icon: Settings,
    roles: ['admin', 'recruiter', 'hr'],
  },
];

export function Sidebar() {
  const location = useLocation();
  const { user } = useAuth();
  const filteredNavItems = navItems.filter((item) =>
    item.roles.includes(user?.roleName.toLowerCase() || '')
  );

  return (
    <aside className="w-64 bg-white shadow-lg min-h-screen border-r-2 border-green-100">
      <nav className="mt-8 px-4 space-y-2">
        {filteredNavItems.map((item) => {
          const Icon = item.icon;
          const isActive = location.pathname === item.path;

          return (
            <Link
              key={item.path}
              to={item.path}
              className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-all ${
                isActive
                  ? 'bg-gradient-to-r from-green-400 to-green-500 text-white shadow-md'
                  : 'text-gray-700 hover:bg-green-50'
              }`}
            >
              <Icon className="w-5 h-5" />
              <span className="font-medium">{item.name}</span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
