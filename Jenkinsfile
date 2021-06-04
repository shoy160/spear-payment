pipeline {
  agent any
  stages {
    stage('build') {
      steps {
        sh 'dotnet restore ${ACB_PROJECT}/${ACB_PROJECT}.csproj'
        sh 'dotnet build ${ACB_PROJECT}/${ACB_PROJECT}.csproj'
      }
    }
    stage('publish') {
      steps {
        sh 'dotnet publish **/${ACB_PROJECT}.csproj -c Release -o ./bin/publish'
      }
    }
    stage('docker') {
      steps {        
        sh 'docker build -t ${ACB_IMAGE_NAME}:3.0_temp ./bin/publish'
        sh '(docker rm -f ${ACB_IMAGE_NAME} && docker rmi ${ACB_IMAGE_NAME}:3.0) || echo \'image \'${ACB_IMAGE_NAME}\' not exist\''
        sh 'docker tag ${ACB_IMAGE_NAME}:3.0_temp ${ACB_IMAGE_NAME}:3.0 && docker rmi ${ACB_IMAGE_NAME}:3.0_temp'
        sh 'docker run --name=${ACB_IMAGE_NAME} --restart=always --network=icb_net -e ACB_MODE=${ACB_MODE} -p ${ACB_DOCKER_PORT}:5000 -d ${ACB_IMAGE_NAME}:3.0'
      }
    }
    stage('push') {
      steps {
        sh 'docker tag ${ACB_IMAGE_NAME}:3.0 ${ACB_DOCKER_HOST}:5000/web/${ACB_IMAGE_NAME} && docker push ${ACB_DOCKER_HOST}:5000/web/${ACB_IMAGE_NAME} && docker rmi ${ACB_DOCKER_HOST}:5000/web/${ACB_IMAGE_NAME}'
      }
    }
  }
  environment {
    ACB_PROJECT = 'Spear.Gateway.Payment'
    ACB_IMAGE_NAME = 'gateway-payment'
    ACB_MODE = 'Test'
    ACB_DOCKER_HOST = 'docker.dev'
    ACB_DOCKER_PORT = 6308
  }
}