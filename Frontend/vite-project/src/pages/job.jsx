import { useEffect, useState } from 'react';
import { Card } from '../components/common/Card';
import { Button } from '../components/common/button';
import { Modal } from '../components/common/modal';
import { Input } from '../components/common/input';
import { Select } from '../components/common/select';
import { showToast } from '../components/common/toast';
import { Briefcase, Plus, Search, Filter, Edit, Pause, CheckCircle, XCircle } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export function Jobs() {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [statusFilter, setStatusFilter] = useState('all');
    const [formData, setFormData] = useState({
        title: '',
        description: '',
        status: 'Open',
        location: '',
        type: 'Full Time',
        baseSalary: 0,
        maxSalary: 0,
        rounds: 1,
        skillIds: []
    });
    const [jobs, setJobs] = useState([]);
    const { user } = useAuth()
    const [availableSkills, setAvailableSkills] = useState([]);
    const [selectedSkills, setSelectedSkills] = useState([]);
    const handleSkillSelect = (skillId) => {
        if (!skillId) return;
        const skill = availableSkills.find(s => s.skillId === parseInt(skillId));

        if (skill && !formData.skillIds.includes(skill.skillId)) {
            setFormData({
                ...formData,
                skillIds: [...formData.skillIds, skill.skillId]
            });
            setSelectedSkills([...selectedSkills, skill]);
        }
    }

    const removeSkill = (skillId) => {
        setFormData({
            ...formData,
            skillIds: formData.skillIds.filter(id => id !== skillId)
        });
        setSelectedSkills(selectedSkills.filter(s => s.skillId !== skillId));
    };

    useEffect(() => {
        const getSkill = async () => {
            try {

                const ret = await fetch(import.meta.env.VITE_API_URI + '/skill', {
                    headers: {
                        'Content-Type': 'application/json',
                        // 'Authorization': `Bearer ${user.jwtToken}`
                    }
                })
                const res = await ret.json();
                if (!ret.ok)
                    new Error(res.message);
                console.log(res)
                setAvailableSkills(res);

                const ret2 = await fetch(import.meta.env.VITE_API_URI + '/position',{
                    headers: {
                        'Content-Type': 'application/json',
                        // 'Authorization': `Bearer ${user.jwtToken}`
                    }
                })
                const res2 = await ret2.json();
                if(!ret2.ok)
                    new Error(res2.message);
                console.log(res2)
                setJobs(res2)
            } catch (e) {
                console.error(error);
                showToast.error('Error', 'Something went wrong during fetch skills');
            }
        }
        // fetch(import.meta.env.VITE_API_URI + '/skill')
        //     .then(res => res.json())
        //     .then(data => {
        //         setAvailableSkills(data);
        //     });
        getSkill();
    }, []);

    const handleCreateJob = async (e) => {
        e.preventDefault();

        const positionData = {
            ...formData,
            recruiterId: user.userId
        };
        console.log(positionData)
        console.log(user)

        try {
            const response = await fetch(import.meta.env.VITE_API_URI + '/position', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                },
                body: JSON.stringify(positionData)
            });

            if (!response.ok) {
                throw new Error('Failed to create job');
            }

            const result = await response.json();
            console.log(result);
            showToast.success('Job Created', 'New job opening has been created successfully');
            setIsCreateModalOpen(false);
        } catch (error) {
            console.error(error);
            showToast.error('Error', 'Something went wrong while creating the job');
        }

    }
    //statik data for tamplet
    // const jobs = [
    //     { id: '1', title: 'Senior Full Stack Developer', department: 'Engineering', location: 'Remote', status: 'open', applicants: 45, createdDate: '2024-01-15' },
    //     { id: '2', title: 'UI/UX Designer', department: 'Design', location: 'New York', status: 'open', applicants: 32, createdDate: '2024-01-18' },
    //     { id: '3', title: 'DevOps Engineer', department: 'Engineering', location: 'San Francisco', status: 'hold', applicants: 28, createdDate: '2024-01-10' },
    //     { id: '4', title: 'Product Manager', department: 'Product', location: 'Boston', status: 'open', applicants: 56, createdDate: '2024-01-20' },
    //     { id: '5', title: 'Data Scientist', department: 'Data', location: 'Remote', status: 'closed', applicants: 41, createdDate: '2024-01-05' },
    // ];

    const getStatusBadge = (status) => {
        const styles = {
            Open: 'bg-green-100 text-green-700 border-green-200',
            Hold: 'bg-yellow-100 text-yellow-700 border-yellow-200',
            Closed: 'bg-gray-100 text-gray-700 border-gray-200',
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
                                <tr key={job.positionId} className="hover:bg-gray-50 transition-colors">
                                    <td className="px-6 py-4">
                                        <div className="flex items-center text-center space-x-3">
                                            <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-green-500 rounded-lg flex items-center justify-center">
                                                <Briefcase className="w-5 h-5 text-white" />
                                            </div>
                                            <div>
                                                <p className="font-semibold text-gray-800">{job.title}</p>
                                                <p className="text-sm text-gray-500">{job.recruiterName}</p>
                                            </div>
                                        </div>
                                    </td>
                                    <td className="text-center  py-4 text-gray-700">{job.type}</td>
                                    <td className="text-center  py-4 text-gray-700">{job.location}</td>
                                    <td className="text-center  py-4">{getStatusBadge(job.status)}</td>
                                    <td className="text-center  py-4">
                                        <span className="font-semibold text-gray-800">{job.applicants   }</span>
                                    </td>
                                    <td className="text-center py-4 text-gray-600 text-sm">{new Date(job.createdAt).toLocaleDateString('en-GB')}</td>
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
                    <Input
                        label="Position Title"
                        placeholder="e.g. Senior Full Stack Developer"
                        value={formData.title}
                        onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                        required
                    />
                    <div className="grid grid-cols-4 gap-4">
                        {/* Status */}
                        <Select
                            label="Status"
                            value={formData.status}
                            onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                            options={[
                                { value: 'Open', label: 'Open' },
                                { value: 'On Hold', label: 'On Hold' },
                                { value: 'Closed', label: 'Closed' },
                            ]}
                        />
                        <Select
                            label="Type"
                            value={formData.type}
                            onChange={(e) => setFormData({ ...formData, type: e.target.value })}
                            options={[
                                { value: 'Full Time', label: 'Full Time' },
                                { value: 'Part Time', label: 'Part Time' },
                                { value: 'Contract', label: 'Contract' },
                            ]}
                        />

                        <Input
                            type="number"
                            label="Base Salary"
                            min="1"
                            value={formData.baseSalary}
                            onChange={(e) => setFormData({ ...formData, baseSalary: parseInt(e.target.value) })}
                            required
                        />
                        <Input
                            type="number"
                            label="Max Salary"
                            min="1"
                            value={formData.maxSalary}
                            onChange={(e) => setFormData({ ...formData, maxSalary: parseInt(e.target.value) })}
                            required
                        />

                    </div>
                    <div className="grid grid-cols-4 gap-4">
                        <div className="col-span-3">
                            <Input
                                label="Location"
                                placeholder="e.g. Remote, Ahmedabad"
                                value={formData.location}
                                onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                                required />
                        </div>
                        <div className="col-span-1">
                            <Input
                                type="number"
                                label="Interview Rounds"
                                min="1"
                                value={formData.rounds}
                                onChange={(e) => setFormData({ ...formData, rounds: parseInt(e.target.value) })}
                                required
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Required Skills</label>
                        <Select
                            value=""
                            onChange={(e) => handleSkillSelect(e.target.value)}
                            options={[
                                { value: '', label: 'Select a skill...' },
                                ...availableSkills.map((s) => ({ value: s.skillId, label: s.skillName }))
                            ]}
                        />

                        {/* Selected Skills Tags */}
                        <div className="flex flex-wrap gap-2 mt-3">
                            {selectedSkills.map((skill) => (
                                <span key={skill.skillId} className="inline-flex items-center bg-green-50 text-green-700 text-sm px-3 py-1 rounded-full border border-green-200">
                                    {skill.skillName}
                                    <button
                                        type="button"
                                        onClick={() => removeSkill(skill.skillId)}
                                        className="ml-2 text-green-500 hover:text-green-800"
                                    >
                                        <XCircle className="w-4 h-4" />
                                    </button>
                                </span>
                            ))}
                        </div>
                    </div>

                    {/* Description */}
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Job Description</label>
                        <textarea
                            rows={4}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
                            placeholder="Enter job description..."
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                            required
                        ></textarea>
                    </div>

                    <div className="flex justify-end space-x-3 pt-4">
                        <Button variant="secondary" onClick={() => setIsCreateModalOpen(false)}>
                            Cancel
                        </Button>
                        <Button type="submit">Create Position</Button>
                    </div>
                </form>
            </Modal>
        </div>
    );

}