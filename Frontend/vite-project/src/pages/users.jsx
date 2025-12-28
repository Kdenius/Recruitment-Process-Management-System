import { useEffect, useState } from 'react';
import { Card } from '../components/common/Card';
import { Button } from '../components/common/button';
import { Modal } from '../components/common/modal';
import { Input } from '../components/common/input';
import { Select } from '../components/common/select';
import { showToast } from '../components/common/toast';
import { Plus, Search, Edit, Trash2, Loader2, Loader, LoaderCircleIcon, Loader2Icon, LoaderPinwheel, User } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export function Users() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectUser, setSelectUser] = useState(null);
  const [users, setUsers] = useState('');
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const { user } = useAuth();
  const [selectedRole, setSelectedRole] = useState('')

  useEffect(() => {
    const fetchUsersAndRoles = async () => {
      try {
        const ret = await fetch(import.meta.env.VITE_API_URI + '/user', {
          headers: {
            'Authorization': `Bearer ${user.jwtToken}`,
            'Content-Type': 'application/json'
          },
        })
        const res = await ret.json();
        if (!ret.ok)
          throw new Error(res.message);
        console.log(res)
        setUsers(res)
        ////
        const ret2 = await fetch(import.meta.env.VITE_API_URI + '/role', {
          headers: {
            'Authorization': `Bearer ${user.jwtToken}`,
            'Content-Type': 'application/json'
          }
        })
        const res2 = await ret2.json();
        if (!ret2.ok)
          throw new Error(res2.message)
        console.log(res2);
        setRoles(res2)


      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }
    fetchUsersAndRoles();
  }, []);


  const handleChangeRole = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const ret = await fetch(`${import.meta.env.VITE_API_URI}/user/update-role`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${user.jwtToken}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          userId: selectUser.userId,
          roleId: selectedRole,
        }),
      });

      const res = await ret.json();
      if (!ret.ok)
        throw new Error(res.message)
      const newRole = roles.find(r => r.roleId == selectedRole);
      setUsers(prev => prev.map(u => u.userId == selectUser.userId ? { ...u, roleId: newRole.roleId, roleName: newRole.roleName } : u))
      showToast.success('Role Updated', res.message || 'User permissions changed')
    } catch (err) {
      showToast.error('Failed', err.message || 'Defaul- Failed to Update')
    } finally {
      setSelectedRole(null)
      setSelectUser(null)
      setLoading(false)
      setIsModalOpen(false)
    }
  }

  const getRoleBadge = (role) => {
    const styles = {
      admin: 'bg-red-100 text-red-700 border-red-200',
      recruiter: 'bg-blue-100 text-blue-700 border-blue-200',
      hr: 'bg-green-100 text-green-700 border-green-200',
      interviewer: 'bg-purple-100 text-purple-700 border-purple-200',
      reviewer: 'bg-yellow-100 text-yellow-700 border-yellow-200',
      viewer: 'bg-gray-100 text-gray-700 border-gray-200',
    };
    return (
      <span className={`inline-flex px-3 py-1 rounded-full text-xs font-medium border ${styles[role.toLowerCase()]}`}>
        {role.charAt(0).toUpperCase() + role.slice(1)}
      </span>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center">
        <Loader
          className={`h-10 w-10 text-green-500 animate-spin`}
          aria-label="Loading..." // Accessibility best practice
        />
      </div>
    );
  }
  return (
    <div className="space-y-6">
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
          {error}
        </div>
      )}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-800">User Management</h1>
          <p className="text-gray-600 mt-1">Manage system users and their roles</p>
        </div>
        <Button onClick={() => setIsModalOpen(true)} className="flex items-center space-x-2">
          <Plus className="w-5 h-5" />
          <span>Add User</span>
        </Button>
      </div>

      <Card>
        <div className="flex items-center space-x-4 mb-6">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              type="text"
              placeholder="Search users..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
            />
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gradient-to-r from-green-50 to-green-100">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">User</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Email</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Role</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Joined Date</th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {users.map((user) => (
                <tr key={user.userId} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-3">
                      <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-green-500 rounded-full flex items-center justify-center">
                        <span className="text-white font-semibold">
                          {user.firstName[0] + user.lastName[0]}
                        </span>
                      </div>
                      <span className="font-semibold text-gray-800">{user.firstName + ' ' + user.lastName}</span>
                    </div>
                  </td>
                  <td className="text-center py-4 text-gray-700">{user.email}</td>
                  <td className="text-center py-4">{getRoleBadge(user.roleName)}</td>
                  <td className="py-4 text-center">
                    <span className="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-700 border border-green-200">
                      Active
                    </span>
                  </td>
                  <td className="py-4 text-center text-gray-600 text-sm">{new Date(user.createdAt).toLocaleDateString('en-GB')}</td>
                  <td className="px-2 py-4">
                    <div className="flex items-center space-x-2">
                      <button onClick={() => { setSelectUser(user); setIsModalOpen(true); }} className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                        <Edit className="w-4 h-4" />
                      </button>
                      <button className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>

      {selectUser && <Modal
        isOpen={isModalOpen}
        onClose={() => { setSelectUser(null); setIsModalOpen(false) }}
        title="Update User's Role"
        size="md"
      >
        <form onSubmit={handleChangeRole} className="space-y-4">
          <div className="flex items-center justify-between p-4 bg-green-50 rounded-lg border border-green-100">
            <div className="flex items-center space-x-3">
              <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-green-500 rounded-full flex items-center justify-center">
                <User className="w-5 h-5 text-white" />
              </div>
              <div>
                <p className="font-semibold text-gray-800">{selectUser.firstName + ' ' + selectUser.lastName}</p>
                <p className="text-sm text-gray-600">{selectUser.email}</p>
              </div>
            </div>
            <div className="text-right">
              <span className="inline-block px-2 py-1 text-xs bg-green-500 text-white rounded-full mt-1">
                {selectUser.roleName}
              </span>
            </div>
          </div>

          <Select
            label="Assign Role"
            options={[
              { value: '', label: 'Select Role' },
              ...roles.map((role) => ({
                value: role.roleId,
                label: role.roleName
              })),

            ]}
            value={selectedRole}
            onChange={(e) => setSelectedRole(e.target.value)}
            required
          />
          <div className="flex justify-end space-x-3 pt-4">
            <Button type="submit" disabled={loading || !selectedRole} >{loading ? 'Updating' : 'Update'}</Button>
            <Button variant="secondary" onClick={() => { setSelectUser(null); setIsModalOpen(false) }}>
              Cancel
            </Button>
          </div>
        </form>
      </Modal>
      }
    </div>
  );
}
