import { useEffect, useState } from 'react';
import { Card } from '../components/common/Card';
import { Button } from '../components/common/button';
import { Modal } from '../components/common/modal';
import { Input } from '../components/common/input';
import { Select } from '../components/common/select';
import { showToast } from '../components/common/toast';
import { Calendar, Plus, Video, MessageSquare, Clock, ArrowRightCircle, CircleArrowLeft, CornerRightDown, CalendarCheck, Star, XCircle, ImageOff } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export function Interviews() {
    const [isScheduleModalOpen, setIsScheduleModalOpen] = useState(false);
    const [isFeedbackModalOpen, setIsFeedbackModalOpen] = useState(false);
    const [interviews, SetInterviews] = useState([]);
    const [selectedInterview, SetSelectedInterview] = useState(null);
    const [availableSkills, setAvailableSkills] = useState([])
    const { user } = useAuth();
    const [ratings, SetRatings] = useState([])
    const [feedback, setFeedback] = useState({
        feedbackText: "",
        feedbackScore: 0
        // ratings: [] // Array of { skillId, rating, remark }
    });


    useEffect(() => {
        const fetchInterview = async () => {
            try {
                const ret = await fetch(import.meta.env.VITE_API_URI + '/interviews/interviewer/' + user.userId, {
                    headers: {
                        'Content-Type': 'application/json',
                        // 'Authorization': `Bearer ${user.jwtToken}`
                    }
                });
                const data = await ret.json();
                // console.log(data)
                if (ret.ok)
                    SetInterviews(data);
                else
                    throw new Error('issue in fetching interviews');


                //fetch skills
                const ret2 = await fetch(import.meta.env.VITE_API_URI + '/skill', {
                    headers: {
                        'Content-Type': 'application/json',
                        // 'Authorization': `Bearer ${user.jwtToken}`
                    }
                })
                const res = await ret2.json();
                if (!ret2.ok)
                    new Error(res.message);
                setAvailableSkills(res);
            } catch (e) {
                console.log(e);
            }

        }
        fetchInterview();
    }, [])

    const updateRating = (skillId, field, value) => {
        SetRatings(prev =>
            prev.map(item =>
                item.skillId === skillId
                    ? { ...item, [field]: value }
                    : item
            )
        );
    };
    const handleSkillSelect = (skillId) => {
        if (!skillId) return;

        // prevent duplicate
        if (ratings.some(r => r.skillId === Number(skillId))) return;

        const skill = availableSkills.find(
            s => s.skillId === Number(skillId)
        );

        if (!skill) return;

        SetRatings(prev => [
            ...prev,
            {
                skillId: skill.skillId,
                skillName: skill.skillName,
                rating: 0,
                remark: ''
            }
        ]);
    }
    const removeSkill = (skillId) => {
        SetRatings(prev =>
            prev.filter(r => r.skillId !== skillId)
        );
    };
    const handleScheduleInterview = (e) => {
        e.preventDefault();
        setIsScheduleModalOpen(false);
        showToast.success(
            'Interview Scheduled',
            'Interview has been scheduled successfully. Meeting invites sent to all participants.'
        );
    };

    const handleSubmitFeedback = async (e) => {
        e.preventDefault();
        const r = ratings.map(r=>({
            skillId: r.skillId,
    rating: r.rating,
    remark: r.remark
        }));
        try{
            const ret = await fetch(import.meta.env.VITE_API_URI+'/interviews/feedback',{
             method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                },
                body: JSON.stringify({
                    interviewId: selectedInterview.interviewId,
                    feedbackText: feedback.feedbackText,
                    feedbackScore: feedback.feedbackScore,
                    ratings: r,
                })
            })

            const res = await ret.json()
            if(!ret.ok)
                throw Error('submition ma locha')
            setIsFeedbackModalOpen(false);
            showToast.success(
                'Feedback Submitted',
                'Interview feedback has been recorded successfully'
            );
        }catch(e){
            console.log(e)
            console.log("falied to submit feedback")
        }
    };

    // const interviews = [
    //     { id: '1', candidate: 'John Doe', position: 'Senior Developer', date: '2024-02-15', time: '10:00 PM', type: 'Technical', round: 1, status: 'scheduled', interviewer: 'Alice Johnson' },
    //     { id: '2', candidate: 'Jane Smith', position: 'UI/UX Designer', date: '2024-02-15', time: '2:00 PM', type: 'HR', round: 2, status: 'completed', interviewer: 'Bob Williams' },
    //     { id: '3', candidate: 'Michael Brown', position: 'DevOps Engineer', date: '2024-02-15', time: '11:00 AM', type: 'Technical', round: 1, status: 'scheduled', interviewer: 'Carol Davis' },

    // ];

    const getStatusBadge = (status) => {
        const styles = {
            scheduled: 'bg-blue-100 text-blue-700 border-blue-200',
            completed: 'bg-green-100 text-green-700 border-green-200',
            cancelled: 'bg-red-100 text-red-700 border-red-200',
        };

        return (
            <span className={`inline-flex px-2 py-1 rounded-full text-xs font-medium border ${styles[status]}`}>
                {status.charAt(0).toUpperCase() + status.slice(1)}
            </span>
        );
    };

    const getTypeBadge = (type) => {
        const styles = {
            Technical: 'bg-purple-100 text-purple-700',
            HR: 'bg-orange-100 text-orange-700',
        };

        return (
            <span className={`inline-flex px-2 py-1 rounded-full text-xs font-medium ${styles[type]}`}>
                {type}
            </span>
        );
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold text-gray-800">Interviews</h1>
                    <p className="text-gray-600 mt-1">Schedule and manage candidate interviews</p>
                </div>
                <Button onClick={() => setIsScheduleModalOpen(true)} className="flex items-center space-x-2">
                    <Plus className="w-5 h-5" />
                    <span>Schedule Interview</span>
                </Button>
            </div>

            <div>
                <Card title="Today's Schedule">
                    {/* Fixed height + scroll */}
                    <div className="h-80 overflow-y-auto">
                        {/* 3 items per row */}
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                            {interviews
                                .filter(i => i.date === '2024-02-15')
                                .map((interview) => (
                                    <div
                                        key={interview.id}
                                        className="p-4 bg-gradient-to-r from-green-50 to-green-100 rounded-lg border border-green-200"
                                    >
                                        <div className="flex items-center justify-between mb-2">
                                            <span className="text-lg font-bold text-green-700">
                                                {interview.time}
                                            </span>
                                            {getTypeBadge(interview.type)}
                                        </div>

                                        <p className="font-semibold text-gray-800">
                                            {interview.candidate}
                                        </p>
                                        <p className="text-sm text-gray-600">
                                            {interview.position}
                                        </p>

                                        <div className="mt-3 flex items-center space-x-2">
                                            <Clock className="w-4 h-4 text-gray-500" />
                                            <span className="text-xs text-gray-600">
                                                Round {interview.round}
                                            </span>
                                        </div>
                                    </div>
                                ))}
                        </div>
                    </div>
                </Card>


                <Card className="lg:col-span-2 mt-10">
                    <div className="overflow-x-auto h-100 overflow-y-auto">
                        <table className="w-full">
                            <thead className="bg-gradient-to-r from-green-50 to-green-100">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Candidate</th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Position</th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Date & Time</th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Type</th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Round</th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Status</th>
                                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase">Actions</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-gray-200">
                                {interviews.map((interview) => (
                                    <tr key={interview.interviewId} className="hover:bg-gray-50">
                                        <td className="px-6 py-4">
                                            <div className="flex items-center space-x-3">
                                                <div>
                                                    <p className="font-semibold">{interview.interviewRound.candidateApplication.candidate.name}</p>
                                                    {/* <p className="text-xs text-gray-500">{interview.interviewer}</p> */}
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">{interview.interviewRound.candidateApplication.position.title}</td>
                                        <td className="px-6 py-4">
                                            <p>{new Date(interview.scheduledAt).toLocaleDateString('en-GB', {
                                                hour: 'numeric',
                                                minute: '2-digit',
                                                hour12: true
                                            })}</p>
                                        </td>
                                        <td className="px-6 py-4">{getTypeBadge(interview.interviewRound.roundType.typeName)}</td>
                                        <td className="px-6 py-4 text-center">{interview.interviewRound.roundNumber}</td>
                                        <td className="px-6 py-4">{getStatusBadge(interview.interviewRound.isCompleted ? 'completed' : 'scheduled')}</td>
                                        <td className="px-6 py-4">
                                            {interview.interviewRound.isCompleted == true && <CalendarCheck className="w-4 h-4 text-blue-600" />}
                                            {interview.interviewRound.isCompleted === false && (
                                                <CalendarCheck
                                                    className="w-4 h-4 text-green-600 cursor-pointer"
                                                    onClick={() => SetSelectedInterview(interview)}
                                                />
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </Card>
            </div>

            {selectedInterview && (

                <Modal
                    isOpen={true}
                    onClose={() => SetSelectedInterview(null)}
                    title={'Interview Evaluation • ' + selectedInterview.interviewRound.candidateApplication.position.title}
                >
                    <div className="space-y-6 max-h-[80vh] overflow-y-auto pr-2">
                        <div className="flex items-start justify-between border-b pb-4">
                            <div>
                                <h3 className="text-xl font-bold text-gray-800">{selectedInterview.interviewRound.candidateApplication.candidate.name}</h3>
                                <p className="text-sm text-gray-600">{selectedInterview.interviewRound.candidateApplication.candidate.email}</p>
                            </div>
                            <div className="text-2xl font-bold text-blue-600">Evaluation</div>
                        </div>

                        {/* EVALUATION SECTION */}
                        <div className="space-y-4 bg-gray-50 p-4 rounded-xl border border-gray-200">
                            <h4 className="font-bold text-gray-800 flex items-center space-x-2">
                                <Star className="w-5 h-5 text-yellow-500 fill-yellow-500" />
                                <span>Candidate Feedback</span>
                            </h4>

                            {/* Overall Score & Text */}
                            <div className="grid grid-cols-1 gap-4">
                                <div>
                                    {/* <label className="block text-sm font-medium text-gray-700 mb-1">Overall Feedback Score (1-10) *</label> */}
                                    <Input
                                        label="Overall Feedback Score (1-10)"
                                        type="number"
                                        min="1" max="10"
                                        // className="w-full p-2 border rounded-md"
                                        value={feedback.feedbackScore}
                                        onChange={(e) => setFeedback(feedback => ({ ...feedback, feedbackScore: parseInt(e.target.value) }))}
                                        placeholder="Score out of 10"
                                        required
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">Feedback Summary *</label>
                                    <textarea
                                        className="w-full p-2 border rounded-md h-24"
                                        value={feedback.feedbackText}
                                        onChange={(e) => setFeedback(feedback => ({ ...feedback, feedbackText: e.target.value }))}
                                        placeholder="Detailed notes about the candidate..."
                                    />
                                </div>
                            </div>

                            {/* Skill Based Ratings */}
                            <div className="mt-6">
                                <label className="block text-sm font-bold text-gray-700 mb-3">
                                    Skill Wise Evaluation
                                </label>

                                {/* Skill Selector */}
                                <Select
                                    value=""
                                    onChange={(e) => handleSkillSelect(e.target.value)}
                                    options={[
                                        { value: '', label: 'Select a skill...' },
                                        ...availableSkills.map(s => ({
                                            value: s.skillId,
                                            label: s.skillName
                                        }))
                                    ]}
                                />

                                {/* Selected Skills List */}
                                <div className="space-y-4 mt-4">
                                    {ratings.map(skill => (
                                        <div
                                            key={skill.skillId}
                                            className="p-4 border rounded-lg bg-green-50"
                                        >
                                            <div className="flex justify-between items-center mb-3">
                                                <span className="font-semibold text-green-800">
                                                    {skill.skillName}
                                                </span>

                                                <button
                                                    type="button"
                                                    onClick={() => removeSkill(skill.skillId)}
                                                    className="text-red-500 hover:text-red-700"
                                                >
                                                    <XCircle className="w-5 h-5" />
                                                </button>
                                            </div>

                                            <div className="grid grid-cols-2 gap-4">
                                                {/* Rating */}
                                                <Input
                                                    label="Rating (1–5)"
                                                    type="number"
                                                    min={1}
                                                    max={5}
                                                    value={skill.rating}
                                                    onChange={(e) =>
                                                        updateRating(skill.skillId, 'rating', Number(e.target.value))
                                                    }
                                                />

                                                {/* Remark */}
                                                <div>
                                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                                        Remark
                                                    </label>
                                                    <textarea
                                                        rows={2}
                                                        value={skill.remark}
                                                        onChange={(e) =>
                                                            updateRating(skill.skillId, 'remark', e.target.value)
                                                        }
                                                        className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-green-500"
                                                        placeholder="Write feedback..."
                                                    />
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>

                        </div>

                        {/* Footer Buttons */}
                        <div className="border-t pt-4 flex justify-end space-x-3">
                            <button
                                onClick={() => SetSelectedInterview(null)}
                                className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSubmitFeedback}
                                disabled={!feedback.feedbackText || feedback.feedbackScore === 0}
                                className="px-6 py-2 bg-blue-600 text-white rounded-lg font-bold hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                Submit Evaluation
                            </button>
                        </div>
                    </div>
                </Modal>
            )}

            {/* Schedule Modal */}
            <Modal isOpen={isScheduleModalOpen} onClose={() => setIsScheduleModalOpen(false)} title="Schedule Interview">
                <form onSubmit={handleScheduleInterview} className="space-y-4">
                    <Select label="Select Candidate" required />
                    <Select label="Position" required />
                    <div className="grid grid-cols-2 gap-4">
                        <Input label="Date" type="date" required />
                        <Input label="Time" type="time" required />
                    </div>
                    <div className="flex justify-end space-x-3">
                        <Button variant="secondary" onClick={() => setIsScheduleModalOpen(false)}>Cancel</Button>
                        <Button type="submit">Schedule</Button>
                    </div>
                </form>
            </Modal>

            {/* Feedback Modal */}
            <Modal isOpen={isFeedbackModalOpen} onClose={() => setIsFeedbackModalOpen(false)} title="Interview Feedback">
                <form onSubmit={handleSubmitFeedback} className="space-y-4">
                    <textarea className="w-full border rounded-lg p-2" rows={4} />
                    <Select label="Recommendation" required />
                    <div className="flex justify-end space-x-3">
                        <Button variant="secondary" onClick={() => setIsFeedbackModalOpen(false)}>Cancel</Button>
                        <Button type="submit">Submit</Button>
                    </div>
                </form>
            </Modal>
        </div>
    );
}
