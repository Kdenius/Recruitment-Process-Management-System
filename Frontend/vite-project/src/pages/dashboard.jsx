import { Card } from '../components/common/Card';
import {
    Briefcase,
    Users,
    Calendar,
    CheckCircle,
    Clock,
    TrendingUp,
    Award,
    Target,
} from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export function Dashboard() {
    const { user } = useAuth();

    const stats = [
        {
            label: 'Open Positions',
            value: '24',
            icon: Briefcase,
            color: 'from-blue-400 to-blue-500',
            change: '+3 this week',
        },
        {
            label: 'Total Candidates',
            value: '1,247',
            icon: Users,
            color: 'from-green-400 to-green-500',
            change: '+145 this month',
        },
        {
            label: 'Interviews Scheduled',
            value: '36',
            icon: Calendar,
            color: 'from-purple-400 to-purple-500',
            change: '12 this week',
        },
        {
            label: 'Offers Made',
            value: '8',
            icon: CheckCircle,
            color: 'from-yellow-400 to-yellow-500',
            change: '5 pending',
        },
    ];

    const recentActivity = [
        {
            type: 'interview',
            text: 'Interview scheduled with John Doe for Senior Developer position',
            time: '2 hours ago',
        },
        {
            type: 'candidate',
            text: 'New candidate profile added: Jane Smith',
            time: '4 hours ago',
        },
        {
            type: 'job',
            text: 'New job posting created: Full Stack Developer',
            time: '1 day ago',
        },
        {
            type: 'offer',
            text: 'Offer accepted by Michael Brown',
            time: '2 days ago',
        },
    ];

    const upcomingInterviews = [
        {
            candidate: 'Sarah Johnson',
            position: 'Frontend Developer',
            time: 'Today at 2:00 PM',
            type: 'Technical',
        },
        {
            candidate: 'David Lee',
            position: 'DevOps Engineer',
            time: 'Today at 4:30 PM',
            type: 'HR',
        },
        {
            candidate: 'Emily Chen',
            position: 'UI/UX Designer',
            time: 'Tomorrow at 10:00 AM',
            type: 'Technical',
        },
    ];

    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-3xl font-bold text-gray-800 mb-2">
                    Welcome back, {user && user.firstName}!
                </h1>
                <p className="text-gray-600">
                    Here's what's happening in your recruitment pipeline
                </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {stats.map((stat, index) => {
                    const Icon = stat.icon;
                    return (
                        <div
                            key={index}
                            className="bg-white rounded-xl shadow-md p-6 border border-gray-200 hover:shadow-lg transition-shadow"
                        >
                            <div className="flex items-center justify-between mb-4">
                                <div
                                    className={`w-12 h-12 bg-gradient-to-r ${stat.color} rounded-lg flex items-center justify-center`}
                                >
                                    <Icon className="w-6 h-6 text-white" />
                                </div>
                                <TrendingUp className="w-5 h-5 text-green-500" />
                            </div>
                            <h3 className="text-2xl font-bold text-gray-800 mb-1">
                                {stat.value}
                            </h3>
                            <p className="text-sm text-gray-600 mb-2">{stat.label}</p>
                            <p className="text-xs text-green-600 font-medium">
                                {stat.change}
                            </p>
                        </div>
                    );
                })}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <Card title="Recent Activity">
                    <div className="space-y-4">
                        {recentActivity.map((activity, index) => (
                            <div
                                key={index}
                                className="flex items-start space-x-3 pb-4 border-b last:border-b-0"
                            >
                                <div className="w-2 h-2 bg-green-500 rounded-full mt-2" />
                                <div className="flex-1">
                                    <p className="text-sm text-gray-800">{activity.text}</p>
                                    <p className="text-xs text-gray-500 mt-1">
                                        {activity.time}
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                </Card>

                <Card title="Upcoming Interviews">
                    <div className="space-y-4">
                        {upcomingInterviews.map((interview, index) => (
                            <div
                                key={index}
                                className="flex items-center justify-between p-4 bg-green-50 rounded-lg border border-green-100"
                            >
                                <div className="flex items-center space-x-3">
                                    <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-green-500 rounded-full flex items-center justify-center">
                                        <Calendar className="w-5 h-5 text-white" />
                                    </div>
                                    <div>
                                        <p className="font-semibold text-gray-800">
                                            {interview.candidate}
                                        </p>
                                        <p className="text-sm text-gray-600">
                                            {interview.position}
                                        </p>
                                    </div>
                                </div>
                                <div className="text-right">
                                    <p className="text-sm font-medium text-gray-800">
                                        {interview.time}
                                    </p>
                                    <span className="inline-block px-2 py-1 text-xs bg-green-500 text-white rounded-full mt-1">
                                        {interview.type}
                                    </span>
                                </div>
                            </div>
                        ))}
                    </div>
                </Card>
            </div>

            <Card title="Quick Stats">
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <Stat icon={Target} label="Time to Hire" value="18 days" note="2 days faster" />
                    <Stat icon={Award} label="Offer Accept Rate" value="87%" note="Above average" />
                    <Stat icon={Clock} label="Interview to Offer" value="72%" note="Strong conversion" />
                    <Stat icon={CheckCircle} label="Positions Filled" value="156" note="This year" />
                </div>
            </Card>
        </div>
    );
}

function Stat({ icon: Icon, label, value, note }) {
    return (
        <div className="p-4 bg-gray-50 rounded-lg border">
            <div className="flex items-center space-x-2 mb-2">
                <Icon className="w-5 h-5 text-gray-600" />
                <span className="text-sm font-medium text-gray-700">{label}</span>
            </div>
            <p className="text-2xl font-bold text-gray-800">{value}</p>
            <p className="text-xs text-green-600 mt-1">{note}</p>
        </div>
    );
}
