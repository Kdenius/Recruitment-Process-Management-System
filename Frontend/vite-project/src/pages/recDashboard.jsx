import { useEffect, useState } from "react";
import { Briefcase, Calendar, CheckCircle2, Plus } from "lucide-react";
import { Modal } from "../components/common/modal";
import { Button } from "../components/common/button";
import { Select } from "../components/common/select";
import { Input } from "../components/common/input";
import { showToast } from "../components/common/toast";

// Example applications
const mockApplications = [
    {
        applicationId: 1,
        candidateName: "John Doe",
        position: "Frontend Developer",
        status: "Interview",
        createdAt: "2025-01-01",
        interviews: [
            {
                interviewId: 101,
                roundNumber: 1,
                roundType: "HR",
                result: "Pass",
                details: {
                    feedbackScore: 8,
                    feedbackText: "Good communication skills",
                    ratings: [
                        { skill: "Communication", rating: 4, remark: "Clear speaking" },
                        { skill: "Culture Fit", rating: 5, remark: "Very compatible" }
                    ]
                }
            },
            {
                interviewId: 102,
                roundNumber: 2,
                roundType: "Technical",
                result: "Pending",
                details: {
                    feedbackScore: null,
                    feedbackText: "",
                    ratings: []
                }
            }
        ]
    }
];

export function RecruiterDashboard() {
    const [applications, setApplications] = useState([])
    const [selectedApplication, setSelectedApplication] = useState(null);
    const [actionPopup, setActionPopup] = useState(null);
    const [remarks, setRemarks] = useState('');
    const [round, setRound] = useState([]);
    const [interviewers, SetInterviewers] = useState([]);
    const [scheduleForm, setScheduleForm] = useState({
        roundTypeId: '',
        scheduledDate: '',
        scheduledTime: '',
        mode: '',
        location: '',
        meetingLink: '',
        interviewerId: ''
    });


    const fetchApplication = async () => {
        try {
            const ret = await fetch(import.meta.env.VITE_API_URI + '/application/applications-with-interviews', {
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                }
            });
            const res = await ret.json();
            if (!ret.ok)
                throw Error(ret.message)
            setApplications(res);
            console.log(res)
        } catch (e) {
            console.error(e)
        }
    }
    const fetchRoundandInter = async () => {
        try {
            const ret = await fetch(import.meta.env.VITE_API_URI + '/user/interviewers', {
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                }
            });
            const data = await ret.json();
            if (!ret.ok) {
                new Error(data.message)
            }
            SetInterviewers(data);

            const ret2 = await fetch(import.meta.env.VITE_API_URI + '/round-types', {
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${user.jwtToken}`
                }
            })
            const data2 = await ret2.json();
            console.log(data2)
            if (!ret2.ok) {
                new Error(data2.message)
            }
            setRound(data2);
        } catch (e) {
            console.log(e)
        }
    }
    useEffect(() => {
        fetchApplication();
        fetchRoundandInter();
    }, [])

const handleAction = async (status) => {
    try {
      const ret = await fetch(import.meta.env.VITE_API_URI + '/application/update-status/' + selectedApplication.applicationId, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          status: status,
          details: remarks
        }),
      })
      const data = await ret.json();
      if (!ret.ok) {
        throw new Error(data.message)
      }
      setActionPopup(null);
      setRemarks('');
      showToast.success('Successfully '+status, data.message)
    } catch (e) {
      console.error(e.message)
    }
  }

  const handleScheduleInterview = async (e) => {
      e.preventDefault();
      try {
        const ret = await fetch(import.meta.env.VITE_API_URI + '/interviews/schedule', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            // 'Authorization': `Bearer ${user.jwtToken}`
          },
          body: JSON.stringify({
            applicationId: Number(selectedApplication.applicationId),
            roundTypeId: Number(scheduleForm.roundTypeId),
            roundNumber: 1,
            scheduledAt: new Date(
              `${scheduleForm.scheduledDate}T${scheduleForm.scheduledTime}`
            ).toISOString(),
            mode: Number(scheduleForm.mode),
            location: scheduleForm.location,
            meetingLink: scheduleForm.meetingLink,
            interviewerIds: [
              Number(scheduleForm.interviewerId)
            ]
          })
        })
        if (!ret.ok)
          throw new Error(ret)
        showToast.success('Scheduled', 'Interview successfully Scheduled');
      } catch (e) {
        console.log(e);
      } finally {
        setActionPopup(null);
        setSelectedApplication(null)
        setScheduleForm({
          roundTypeId: '',
          scheduledDate: '',
          scheduledTime: '',
          mode: '',
          location: '',
          meetingLink: '',
          interviewerId: ''
        })
      }
    }

    const getStatusColor = status => {
        if (status === "Interview") return "text-green-600";
        if (status === "Shortlisted") return "text-blue-600";
        if (status === "Screening") return "text-yellow-600";
        if (status === "Rejected") return "text-red-600";
        return "text-gray-600";
    };


    return (
        <div className="space-y-6">
            {/* Header */}
            <div>
                <h1 className="text-3xl font-bold text-gray-800">
                    Candidate Applications
                </h1>
                <p className="text-gray-600">
                    Review applications and interview feedback
                </p>
            </div>

            {/* Cards Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {applications.map(app => (
                    <div
                        key={app.applicationId}
                        onClick={() => setSelectedApplication(app)}
                        className="bg-gradient-to-r from-green-50 to-white rounded-lg border border-gray-200 p-5 hover:shadow-md hover:border-green-300 transition cursor-pointer"
                    >
                        <div className="flex items-start justify-between mb-3">
                            <div>
                                <h3 className="text-lg font-bold text-gray-800">
                                    {app.candidateName}
                                </h3>
                                <p className="text-sm text-gray-600">{app.position}</p>
                            </div>
                            <div
                                className={`text-sm font-bold ${getStatusColor(app.status)}`}
                            >
                                {app.status}
                            </div>
                        </div>

                        <div className="flex flex-wrap gap-4 text-sm text-gray-600 mb-3">
                            <div className="flex items-center gap-1">
                                <Briefcase className="w-4 h-4" />
                                {app.position}
                            </div>
                            <div className="flex items-center gap-1">
                                <Calendar className="w-4 h-4" />
                                {new Date(app.createdAt).toLocaleDateString("en-GB")}
                            </div>
                        </div>

                        <div className="flex items-center gap-2 text-green-600 text-sm font-medium">
                            <CheckCircle2 className="w-4 h-4" />
                            {app.interviews.length} Interview(s)
                        </div>
                    </div>
                ))}
            </div>

            {/* Details Modal */}
            {selectedApplication && (
                <Modal
                    isOpen={true}
                    onClose={() => setSelectedApplication(null)}
                    title={`Application • ${selectedApplication.candidateName}`}
                    size="xl"
                >
                    <div className="space-y-6">
                        {/* Summary */}
                        <div className="bg-green-50 border border-green-200 rounded-lg p-4">
                            <p className="font-semibold text-gray-800">
                                Position:{" "}
                                <span className="font-normal">{selectedApplication.position}</span>
                            </p>
                            <p className="text-sm text-gray-600">Status: {selectedApplication.status}</p>
                        </div>

                        {/* Interviews */}
                        <div className="space-y-4">
                            <h3 className="font-bold text-gray-800 text-lg">
                                Interview Rounds
                            </h3>

                            {selectedApplication.interviews.map(interview => (
                                <div
                                    key={interview.interviewId}
                                    className="border border-gray-200 rounded-lg p-4 bg-white shadow-sm"
                                >
                                    {/* Header */}
                                    <div className="flex justify-between mb-2">
                                        <p className="font-semibold">
                                            Round {interview.roundNumber} – {interview.roundType}
                                        </p>
                                        <span
                                            className={`${interview.result === "Pass"
                                                ? "text-green-700"
                                                : interview.result === "Pending"
                                                    ? "text-gray-500"
                                                    : "text-blue-700"
                                                } font-medium`}
                                        >
                                            {interview.result}
                                        </span>
                                    </div>

                                    {/* Ratings */}
                                    {interview.details.ratings.length > 0 && (
                                        <div className="grid grid-cols-1 md:grid-cols-2 gap-2 mb-2">
                                            {interview.details.ratings.map((r, i) => (
                                                <div key={i} className="p-2 border rounded-md bg-gray-50">
                                                    <div className="flex justify-between">
                                                        <span className="font-medium">{r.skill}</span>
                                                        <span className="text-green-700 font-semibold">
                                                            {r.rating}/5
                                                        </span>
                                                    </div>
                                                    {r.remark && (
                                                        <p className="text-sm text-gray-600 mt-1">
                                                            Remark: {r.remark}
                                                        </p>
                                                    )}
                                                </div>
                                            ))}
                                        </div>
                                    )}

                                    {/* Feedback Score + Remark */}
                                    {interview.details.feedbackScore && (
                                        <div className="p-2 border-t border-gray-200 mt-2 text-sm">
                                            <p>
                                                <span className="font-semibold">Feedback Score:</span>{" "}
                                                {interview.details.feedbackScore}/10
                                            </p>
                                            <p>
                                                <span className="font-semibold">Feedback Remark:</span>{" "}
                                                {interview.details.feedbackText}
                                            </p>
                                        </div>
                                    )}
                                </div>
                            ))}

                            {/* Add Interview Card */}
                            <div
                                onClick={() => {setActionPopup('interview')}}
                                className="flex items-center justify-center border-2 border-dashed border-gray-300 rounded-lg p-4 cursor-pointer hover:bg-gray-50 transition"
                            >
                                <Plus className="w-6 h-6 text-gray-400 mr-2" />
                                <span className="text-gray-500 font-medium">Add Interview</span>
                            </div>
                        </div>

                        {/* Action Buttons */}
                        <div className="flex gap-3 justify-end mt-4">
                            <Button
                                variant="success"
                                onClick={() => setActionPopup("decision")}
                            >
                                Close Application
                            </Button>
                            {/* <Button
                                variant="primary"
                                onClick={() => setActionPopup("Select")}
                            >
                                Select
                            </Button> */}
                        </div>
                    </div>
                </Modal>
            )}

            {/* popup for reject or select  /////////////////////////////*/}
            {actionPopup === 'decision' && <Modal
                isOpen={true}
                onClose={() => {
                    setActionPopup(null);
                    setRemarks('');
                }}
                title="Review Decision"
            >
                <div className="space-y-4">
                    <p className="text-gray-700">
                        Please choose an action for{' '}
                        <strong>{selectedApplication.candidateName}</strong>
                    </p>

                    <textarea
                        value={remarks}
                        onChange={(e) => setRemarks(e.target.value)}
                        placeholder="Add a short note (optional)"
                        className="w-full border border-gray-300 rounded-lg p-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                        rows={3}
                    />

                    <div className="flex justify-end gap-3 pt-2">
                        <button
                            onClick={() => handleAction('Rejected')}
                            className="px-4 py-2 bg-red-500 hover:bg-red-600 text-white rounded-lg text-sm font-medium"
                        >
                            Reject
                        </button>

                        <button
                            onClick={() => handleAction('Selected')}
                            className="px-4 py-2 bg-green-500 hover:bg-green-600 text-white rounded-lg text-sm font-medium"
                        >
                            Select
                        </button>
                    </div>
                </div>
            </Modal>
            }


            {/* // popup for creating new round///////////////////////// */}

            {actionPopup === 'interview' &&
                <Modal
                    isOpen={true}
                    onClose={() => { setActionPopup(''); setSelectedApplication(null) }}
                    title="Schedule Interview"
                    size="lg"
                >
                    <form onSubmit={handleScheduleInterview} className="space-y-5">

                        {/* Candidate & Position (Read Only) */}
                        <div className="bg-green-50 border border-green-200 rounded-lg p-4">
                            <p className="text-sm font-semibold text-gray-800">
                                Candidate: <span className="font-normal">{selectedApplication.candidateName}</span>
                            </p>
                            <p className="text-sm text-gray-600">
                                Position: {selectedApplication.position}
                            </p>
                        </div>

                        {/* Round Type */}
                        <Select
                            label="Round Type"
                            value={scheduleForm.roundTypeId}
                            onChange={(e) => setScheduleForm(prev => ({
                                ...prev,
                                roundTypeId: e.target.value
                            }))}
                            options={[
                                { value: '', label: 'Select round type...' },
                                ...round.map(rt => ({
                                    value: rt.roundTypeId,
                                    label: rt.typeName
                                }))
                            ]}
                            required
                        />

                        {/* Date & Time */}
                        <div className="grid grid-cols-2 gap-4">
                            <Input
                                label="Date"
                                type="date"
                                value={scheduleForm.scheduledDate}
                                onChange={(e) => setScheduleForm(prev => ({
                                    ...prev,
                                    scheduledDate: e.target.value
                                }))}
                                required
                            />
                            <Input
                                label="Time"
                                type="time"
                                value={scheduleForm.scheduledTime}
                                onChange={(e) => setScheduleForm(prev => ({
                                    ...prev,
                                    scheduledTime: e.target.value
                                }))}
                                required
                            />
                        </div>

                        {/* Mode */}
                        <Select
                            label="Interview Mode"
                            value={scheduleForm.mode}
                            onChange={(e) => setScheduleForm(prev => ({
                                ...prev,
                                mode: e.target.value
                            }))}
                            options={[
                                { value: '', label: 'Select mode...' },
                                { value: 0, label: 'Online' },
                                { value: 1, label: 'Offline' }
                            ]}
                            required
                        />

                        {/* Conditional Fields */}
                        {scheduleForm.mode == 1 && (
                            <Input
                                label="Location"
                                placeholder="Enter interview location"
                                value={scheduleForm.location}
                                onChange={(e) => setScheduleForm(prev => ({
                                    ...prev,
                                    location: e.target.value
                                }))}
                                required
                            />
                        )}

                        {scheduleForm.mode == 0 && (
                            <Input
                                label="Meeting Link"
                                placeholder="Enter meeting link"
                                value={scheduleForm.meetingLink}
                                onChange={(e) => setScheduleForm(prev => ({
                                    ...prev,
                                    meetingLink: e.target.value
                                }))}
                                required
                            />
                        )}

                        {/* Interviewer */}
                        <Select
                            label="Interviewer"
                            value={scheduleForm.interviewerId}
                            onChange={(e) => setScheduleForm(prev => ({
                                ...prev,
                                interviewerId: e.target.value
                            }))}
                            options={[
                                { value: '', label: 'Select interviewer...' },
                                ...interviewers.map(i => ({
                                    value: i.userId,
                                    label: i.firstName + ' ' + i.lastName
                                }))
                            ]}
                            required
                        />

                        {/* Actions */}
                        <div className="flex justify-end space-x-3 pt-4">
                            <Button
                                variant="secondary"
                                type="button"
                                onClick={() => setActionPopup(null)}
                            >
                                Cancel
                            </Button>
                            <Button type="submit">
                                Schedule Interview
                            </Button>
                        </div>

                    </form>
                </Modal>
            }

        </div>
    );
}
