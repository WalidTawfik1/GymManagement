/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}",
    "../Gym.UI/**/*.xaml"
  ],
  theme: {
    extend: {
      colors: {
        gym: {
          primary: '#6B46C1', // Purple background
          accent: '#F5B800',  // Gold/Yellow accents
          success: '#059669', // Green
          info: '#0891B2',    // Cyan
          danger: '#DC2626',  // Red
          warning: '#D97706', // Orange
          dark: '#1F2937',    // Dark Gray text
        }
      }
    },
  },
  plugins: [],
}
