# Welcome to the UI Application for my implementations of a Twitter Sampled Volume Stream processor

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

In order to run the React Front-end, Node and npm need to be installed. Both are freely available. I also highly recommend using Node Version Manager for this process, as it makes the process super simple.

- [Node Version Manager](https://github.com/nvm-sh/nvm)
  - [For Windows](https://github.com/coreybutler/nvm-windows)
- [NodeJS](https://nodejs.org/en/) (For reference only, should be installed through NVM)
<br/><br/>

Once you have node running on your system, you need to install the modules for the application before running it locally.

```
cd <Project Path>/src/UI/neal.twitter.reactapp/src
npm i
```
<br/>

Once that completes, you can launch the React application from the same directory using the `start` script below. Once launched, a browser should open to `http://localhost:4300` and if you have the `API`s properly running in a docker container, you will get a UI that shows the currents from each of the `API`s that will refresh approximately every 30 seconds.
<br/><br/>

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:4300](http://localhost:4300) to view it in the browser.

The page will reload if you make edits.\
You will also see any lint errors in the console.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.