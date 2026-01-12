import { useEffect, useState } from 'react';
import { Card } from '../components/common/Card';
import {
  Users,
  CheckCircle,
  Clock,
  Eye,
  UserCheck,
  MapPin,
  Briefcase,
  DollarSign,
  BookOpen,
  CheckCircle2,
  X,
  IndianRupee,
  BaggageClaim,
  BadgeIndianRupee,
  BookAudio,
  File,
  Calendar,
  CalendarArrowDown,
  CalendarArrowUpIcon,
} from 'lucide-react';
import { Modal } from '../components/common/modal';
import { showToast } from '../components/common/toast';
import { useAuth } from '../context/AuthContext';
import { Select } from '../components/common/select';
import { Input } from '../components/common/input';
import { Button } from '../components/common/button';

export function Applications() {
  const [filter, setFilter] = useState('all');
  const [applications, setApplications] = useState([]);
  const [selectedApplication, setSelectedApplication] = useState(null);
  const [actionPopup, setActionPopup] = useState(null); // 'shortlist' | 'reject'
  const [remarks, setRemarks] = useState('');
  const { user } = useAuth();
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
  const candidateSkills =
    selectedApplication?.candidate.skills.map(s => s.skillName.toLowerCase()) || [];

  const requiredSkills =
    selectedApplication?.position.skills.filter(s => s.isRequired) || [];

  const calculateMatchPercentage = () => {
    if (!requiredSkills.length) return 0;

    const matched = requiredSkills.filter(rs =>
      candidateSkills.includes(rs.skillName.toLowerCase())
    ).length;

    return Math.round((matched / requiredSkills.length) * 100);
  };

  useEffect(() => {
    const fetchApplication = async () => {
      try {
        const ret = await fetch(import.meta.env.VITE_API_URI + '/application');
        const data = await ret.json();
        if (!ret.ok)
          throw new Error("Issue in fetching application");
        console.log(data);
        // const genData = data.map(p => ({
        //   ...p,
        //   requiredSkills: p.positionSkills.filter(sk => sk.isRequired).map(sk => sk.roleName),
        //   preferredSkills: p.positionSkills.filter(sk => !sk.isRequired).map(sk => sk.roleName)
        // }))
        // job.positionSkills.filter(sk => sk.isRequired).map(sk => sk.roleName)
        // console.log(genData)
        // setJobs(genData);
        setApplications(data)
      } catch (e) { }
      finally { }
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
    fetchApplication();
    if (user?.roleName == "Recruiter") {
      fetchRoundandInter();
    }
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
      showToast.success('Successfully Shortlisted', data.message)
    } catch (e) {
      console.log(e.message)
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

  const filteredApplications = applications.filter(app => {
    if (filter === 'all') return true;
    return app.status === filter;
  });

  const getStageBadge = (stage) => {
    if (stage === 'Shortlisted') {
      return (
        <span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-700 border border-green-200">
          <CheckCircle className="w-3 h-3" />
          <span>Shortlisted</span>
        </span>
      );
    }
    if(stage == 'Interview'){
      return(<span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-700 border border-blue-200">
          <CalendarArrowUpIcon className="w-3 h-3" />
          <span>Interview</span>
        </span>
      )
    }

    return (
      <span className="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-700 border border-yellow-200">
        <Clock className="w-3 h-3" />
        <span>Screening</span>
      </span>
    );
  };

  const stats = [
    {
      label: 'Total Applications',
      value: applications.length || 'NA',
      icon: Users,
      color: 'from-blue-400 to-blue-500',
    },
    {
      label: 'Shortlisted',
      value: applications.filter(app => app.status != 'Screening').length,
      icon: UserCheck,
      color: 'from-green-400 to-green-500',
    },
    {
      label: 'In Screening',
      value: applications.filter(app => app.status == 'Screening').length,
      icon: Clock,
      color: 'from-yellow-400 to-yellow-500',
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-800">
          Candidate Review
        </h1>
        <p className="text-gray-600 mt-1">
          Review applications and shortlist candidates
        </p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {stats.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <div
              key={index}
              className="bg-white rounded-xl shadow-md p-6 border border-gray-200"
            >
              <div className="flex items-center justify-between mb-4">
                <div
                  className={`w-12 h-12 bg-gradient-to-r ${stat.color} rounded-lg flex items-center justify-center`}
                >
                  <Icon className="w-6 h-6 text-white" />
                </div>
              </div>
              <h3 className="text-2xl font-bold text-gray-800">
                {stat.value}
              </h3>
              <p className="text-sm text-gray-600">{stat.label}</p>
            </div>
          );
        })}
      </div>

      <Card>
        <div className="flex items-center space-x-4 mb-6">
          <button
            onClick={() => setFilter('all')}
            className={`px-4 py-2 rounded-lg font-medium transition-all ${filter === 'all'
              ? 'bg-gradient-to-r from-green-400 to-green-500 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
          >
            All Applications
          </button>

          <button
            onClick={() => setFilter('Screening')}
            className={`px-4 py-2 rounded-lg font-medium transition-all ${filter === 'screening'
              ? 'bg-gradient-to-r from-green-400 to-green-500 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
          >
            Screening
          </button>

          <button
            onClick={() => setFilter('Shortlisted')}
            className={`px-4 py-2 rounded-lg font-medium transition-all ${filter === 'shortlisted'
              ? 'bg-gradient-to-r from-green-400 to-green-500 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
          >
            Shortlisted
          </button>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gradient-to-r from-green-50 to-green-100">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                  Candidate
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                  Position
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                  Applied Date
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                  Stage
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-gray-200">
              {filteredApplications.map(app => (
                <tr
                  key={app.applicationId}
                  className="hover:bg-gray-50 transition-colors"
                >
                  <td className="pl-6 py-4">
                    <div>
                      <p className="font-semibold text-gray-800">{app.candidate.name}</p>
                      <p className="text-sm text-gray-500">{app.candidate.email}</p>
                    </div>
                  </td>

                  <td className="text-center py-4 text-gray-700">
                    {app.position.title}
                  </td>

                  <td className="text-center py-4 text-gray-600 text-sm">
                    {new Date(app.createdAt).toLocaleDateString('en-GB')}
                  </td>

                  <td className="px-6 py-4">
                    {getStageBadge(app.status)}
                  </td>

                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-2">
                      <button className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                        onClick={() => { setSelectedApplication(app); setActionPopup('review') }} >
                        <Eye className="w-4 h-4" />
                      </button>

                      {app.status === 'Screening' && (
                        <button className="p-2 text-green-600 hover:bg-green-50 rounded-lg transition-colors"
                          onClick={() => {
                            setSelectedApplication(app);
                            setActionPopup('decision');
                          }}>
                          <UserCheck className="w-4 h-4" />
                        </button>
                      )}
                      {app.status === 'Shortlisted' && user?.roleName == 'Recruiter' && (
                        <button className="p-2 text-green-600 hover:bg-green-50 rounded-lg transition-colors"
                          onClick={() => {
                            setSelectedApplication(app);
                            setActionPopup('interview');
                          }}>
                          <CalendarArrowUpIcon className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>

      {selectedApplication && actionPopup == 'review' && (
        <Modal
          isOpen={true}
          onClose={() => setSelectedApplication(null)}
          title={'Job Application' + ' • ' + selectedApplication.position.title}
        >
          <div className="space-y-6">

            <div className="flex items-start justify-between">
              <div>
                <h3 className="text-xl font-bold text-gray-800">
                  {selectedApplication.candidate.name}
                </h3>
                <p className="text-sm text-gray-600">
                  {selectedApplication.candidate.phoneNumber} • {selectedApplication.candidate.email}
                </p>
              </div>

              <div
                className={`text-3xl font-bold ${calculateMatchPercentage() >= 75
                  ? 'text-green-600'
                  : calculateMatchPercentage() >= 50
                    ? 'text-yellow-600'
                    : 'text-red-600'
                  }`}
              >
                {calculateMatchPercentage()}% Match
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="flex items-center space-x-2">
                <MapPin className="w-5 h-5 text-blue-600" />
                <span>{selectedApplication.position.location}</span>
              </div>
              <div className="flex items-center space-x-2">
                <Briefcase className="w-5 h-5 text-blue-600" />
                <span>{selectedApplication.position.type}</span>
              </div>
              <div className="flex items-center space-x-2">
                <BadgeIndianRupee className="w-5 h-5 text-blue-600" />
                <span>
                  ₹{selectedApplication.position.baseSalary} - ₹{selectedApplication.position.maxSalary}
                </span>
              </div>
              <a
                href={`${import.meta.env.VITE_API_URI}/${encodeURI(
                  selectedApplication.candidate.resumeUrl
                )}`}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center space-x-2"
              >
                <File className="w-5 h-5 text-blue-600" />
                <span>Check Resume</span>
              </a>
            </div>


            <div className="border-t pt-4">
              <div className="flex flex-wrap gap-2">
                {candidateSkills.map((skill, index) => (
                  <span
                    key={index}
                    className="px-3 py-1 bg-gradient-to-r from-blue-100 to-blue-50 text-blue-700 rounded-full text-sm font-medium border border-blue-200"
                  >
                    {skill}
                  </span>
                ))}
              </div>
              {/* <p className="text-gray-700">
                <strong>Phone:</strong> {selectedApplication.candidate.phoneNumber}
              </p>
              <p className="text-gray-700">
                <strong>Status:</strong> {selectedApplication.status}
              </p> */}
            </div>

            <div>
              <h4 className="font-bold text-gray-800 mb-3 flex items-center space-x-2">
                <BookOpen className="w-5 h-5 text-blue-600" />
                <span>Required Skills</span>
              </h4>

              <div className="space-y-2">
                {requiredSkills.map((skill, index) => {
                  const hasSkill = candidateSkills.includes(
                    skill.skillName.toLowerCase()
                  );

                  return (
                    <div key={index} className="flex items-center space-x-2">
                      {hasSkill ? (
                        <CheckCircle2 className="w-5 h-5 text-green-600" />
                      ) : (
                        <div className="w-5 h-5 rounded border border-gray-300"></div>
                      )}
                      <span
                        className={
                          hasSkill
                            ? 'text-green-700 font-medium'
                            : 'text-gray-700'
                        }
                      >
                        {skill.skillName}
                      </span>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Footer */}
            <div className="border-t pt-4 flex justify-end">
              <button
                onClick={() => setSelectedApplication(null)}
                className="px-4 py-2 bg-gray-200 hover:bg-gray-300 rounded-lg flex items-center space-x-2"
              >
                <X className="w-4 h-4" />
                <span>Close</span>
              </button>
            </div>
          </div>
        </Modal>
      )}

      {actionPopup === 'decision' && selectedApplication && (
        <Modal
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
              <strong>{selectedApplication.candidate.name}</strong>
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
                onClick={() => handleAction('Shortlisted')}
                className="px-4 py-2 bg-green-500 hover:bg-green-600 text-white rounded-lg text-sm font-medium"
              >
                Shortlist
              </button>
            </div>
          </div>
        </Modal>
      )}

      {/* interview module for recruiter///////////////////////////////// */}
      {actionPopup == 'interview' && selectedApplication && (
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
                Candidate: <span className="font-normal">{selectedApplication.candidate.name}</span>
              </p>
              <p className="text-sm text-gray-600">
                Position: {selectedApplication.position.title}
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
      )}


    </div>
  );
}
