import toast, { Toaster } from 'react-hot-toast';
import { CheckCircle, AlertCircle, AlertTriangle, Info, X } from 'lucide-react';

export function ToastContainer() {
  return (
    <Toaster
      position="top-right"
      reverseOrder={false}
      gutter={8}
      toastOptions={{
        duration: 7000,
        style: {
          fontSize: '14px',
          fontFamily: 'system-ui, -apple-system, sans-serif',
        },
      }}
    />
  );
}

export function CustomToast({ t, title, message, type }) {
  const typeConfig = {
    success: {
      bg: 'bg-green-50',
      border: 'border-green-200',
      icon: <CheckCircle className="w-5 h-5 text-green-600" />,
      title: 'text-green-800',
      message: 'text-green-700',
    },
    error: {
      bg: 'bg-red-50',
      border: 'border-red-200',
      icon: <AlertCircle className="w-5 h-5 text-red-600" />,
      title: 'text-red-800',
      message: 'text-red-700',
    },
    warning: {
      bg: 'bg-yellow-50',
      border: 'border-yellow-200',
      icon: <AlertTriangle className="w-5 h-5 text-yellow-600" />,
      title: 'text-yellow-800',
      message: 'text-yellow-700',
    },
    info: {
      bg: 'bg-blue-50',
      border: 'border-blue-200',
      icon: <Info className="w-5 h-5 text-blue-600" />,
      title: 'text-blue-800',
      message: 'text-blue-700',
    },
  };

  const config = typeConfig[type];

  return (
    <div
      className={`${config.bg} border ${config.border} rounded-lg shadow-lg p-4 max-w-sm flex items-start space-x-3 animate-slide-in`}
      style={{
        opacity: t.visible ? 1 : 0,
        transition: 'opacity 0.3s ease-in-out',
      }}
    >
      <div className="flex-shrink-0">{config.icon}</div>
      <div className="flex-1">
        <p className={`font-semibold ${config.title}`}>{title}</p>
        <p className={`text-sm mt-1 ${config.message}`}>{message}</p>
      </div>
      <button
        onClick={() => toast.dismiss(t.id)}
        className="flex-shrink-0 text-gray-400 hover:text-gray-600 transition-colors"
      >
        <X className="w-5 h-5" />
      </button>
    </div>
  );
}

export const showToast = {
  success: (title, message) => {
    toast.custom((t) => <CustomToast t={t} title={title} message={message} type="success" />);
  },
  error: (title, message) => {
    toast.custom((t) => <CustomToast t={t} title={title} message={message} type="error" />);
  },
  warning: (title, message) => {
    toast.custom((t) => <CustomToast t={t} title={title} message={message} type="warning" />);
  },
  info: (title, message) => {
    toast.custom((t) => <CustomToast t={t} title={title} message={message} type="info" />);
  },
};

export default toast;
