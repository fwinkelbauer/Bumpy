pipeline {
    agent any

    triggers {
        pollSCM('')
    }

    options {
        buildDiscarder(logRotator(numToKeepStr: '100'))
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                powershell '''$ErrorActionPreference = "Stop"
                cd ./Source
                ./build.ps1'''
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts 'Artifacts/**/*.nupkg'
            }
        }
    }

    post {
        always {
            deleteDir()
        }
    }
}
