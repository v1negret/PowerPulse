/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{razor,cshtml,html}"],
  darkMode: 'class', // Используем класс для переключения тем
  theme: {
    extend: {},
    screens: {
      'sm': '640px',
      'md': '768px',
      'lg': '1024px',
      'xl': '1280px',
    },
  },
  plugins: [],
}