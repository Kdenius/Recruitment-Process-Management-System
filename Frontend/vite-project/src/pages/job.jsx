import { useState } from 'react';
import { Card } from '../components/common/Card';
import { Button } from '../components/common/button';
import { Modal } from '../components/common/modal';
import { Input } from '../components/common/input';
import { Select } from '../components/common/select';
import { showToast } from '../components/common/toast';
import { Briefcase, Plus, Search, Filter, Edit, Pause, CheckCircle, XCircle } from 'lucide-react';

export function Jobs() {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [statusFilter, setStatusFilter] = useState('all');

    const handleCreateJob = (e) => {
        e.preventDefault();
        setIsCreateModalOpen(false);
        showToast.success('Job Created', 'New job opening has been created successfully');
    };

    const jobs = [
        { id: '1', title: 'Senior Full Stack Developer', department: 'Engineering', location: 'Remote', status: 'open', applicants: 45, createdDate: '2024-01-15' },
        { id: '2', title: 'UI/UX Designer', department: 'Design', location: 'New York', status: 'open', applicants: 32, createdDate: '2024-01-18' },
        { id: '3', title: 'DevOps Engineer', department: 'Engineering', location: 'San Francisco', status: 'hold', applicants: 28, createdDate: '2024-01-10' },
        { id: '4', title: 'Product Manager', department: 'Product', location: 'Boston', status: 'open', applicants: 56, createdDate: '2024-01-20' },
        { id: '5', title: 'Data Scientist', department: 'Data', location: 'Remote', status: 'closed', applicants: 41, createdDate: '2024-01-05' },
    ];

    const getStatusBadge = (status) => {
        const styles = {
            open: 'bg-green-100 text-green-700 border-green-200',
            hold: 'bg-yellow-100 text-yellow-700 border-yellow-200',
            closed: 'bg-gray-100 text-gray-700 border-gray-200',
        };
        const icons = {
            open: <CheckCircle className="w-3 h-3" />,
            hold: <Pause className="w-3 h-3" />,
            closed: <XCircle className="w-3 h-3" />,
        };
        return (
            <span className={`inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium border ${styles[status]}`}>
                {icons[status]}
                <span className="capitalize">{status}</span>
            </span>
        );
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold text-gray-800">Job Openings</h1>
                    <p className="text-gray-600 mt-1">Manage and track all open positions</p>
                </div>
                <Button onClick={() => setIsCreateModalOpen(true)} className="flex items-center space-x-2">
                    <Plus className="w-5 h-5" />
                    <span>Create Job</span>
                </Button>
            </div>

            <Card>
                <div className="flex flex-col sm:flex-row gap-4 mb-6">
                    <div className="flex-1 relative">
                        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                        <input
                            type="text"
                            placeholder="Search jobs..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
                        />
                    </div>
                    <div className="flex items-center space-x-2">
                        <Filter className="w-5 h-5 text-gray-400" />
                        <Select
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value)}
                            options={[
                                { value: 'all', label: 'All Status' },
                                { value: 'open', label: 'Open' },
                                { value: 'hold', label: 'On Hold' },
                                { value: 'closed', label: 'Closed' },
                            ]}
                        />
                    </div>
                </div>

                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gradient-to-r from-green-50 to-green-100">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Position</th>
                                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Department</th>
                                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Location</th>
                                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Status</th>
                                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Applicants</th>
                                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Created</th>
                                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Actions</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {jobs.map((job) => (
                                <tr key={job.id} className="hover:bg-gray-50 transition-colors">
                                    <td className="px-6 py-4">
                                        <div className="flex items-center space-x-3">
                                            <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-green-500 rounded-lg flex items-center justify-center">
                                                <Briefcase className="w-5 h-5 text-white" />
                                            </div>
                                            <div>
                                                <p className="font-semibold text-gray-800">{job.title}</p>
                                                <p className="text-sm text-gray-500">ID: {job.id}</p>
                                            </div>
                                        </div>
                                    </td>
                                    <td className="px-6 py-4 text-gray-700">{job.department}</td>
                                    <td className="px-6 py-4 text-gray-700">{job.location}</td>
                                    <td className="px-6 py-4">{getStatusBadge(job.status)}</td>
                                    <td className="px-6 py-4">
                                        <span className="font-semibold text-gray-800">{job.applicants}</span>
                                    </td>
                                    <td className="px-6 py-4 text-gray-600 text-sm">{job.createdDate}</td>
                                    <td className="px-6 py-4">
                                        <div className="flex items-center space-x-2">
                                            <button className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                                                <Edit className="w-4 h-4" />
                                            </button>
                                            <button className="p-2 text-yellow-600 hover:bg-yellow-50 rounded-lg transition-colors">
                                                <Pause className="w-4 h-4" />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </Card>

            <Modal
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                title="Create New Job Opening"
                size="lg"
            >
                <form onSubmit={handleCreateJob} className="space-y-4">
                    <Input label="Job Title" placeholder="e.g., Senior Full Stack Developer" required />
                    <div className="grid grid-cols-2 gap-4">
                        <Select
                            label="Department"
                            options={[
                                { value: '', label: 'Select Department' },
                                { value: 'engineering', label: 'Engineering' },
                                { value: 'design', label: 'Design' },
                                { value: 'product', label: 'Product' },
                                { value: 'data', label: 'Data' },
                            ]}
                            required
                        />
                        <Select
                            label="Job Type"
                            options={[
                                { value: '', label: 'Select Type' },
                                { value: 'fulltime', label: 'Full Time' },
                                { value: 'parttime', label: 'Part Time' },
                                { value: 'contract', label: 'Contract' },
                            ]}
                            required
                        />
                    </div>
                    <Input label="Location" placeholder="e.g., Remote, New York, etc." required />
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Job Description</label>
                        <textarea
                            rows={4}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
                            placeholder="Enter job description..."
                        ></textarea>
                    </div>
                    <div className="flex justify-end space-x-3 pt-4">
                        <Button variant="secondary" onClick={() => setIsCreateModalOpen(false)}>
                            Cancel
                        </Button>
                        <Button type="submit">Create Job</Button>
                    </div>
                </form>
            </Modal>
        </div>
    );
}

